using System.Runtime.CompilerServices;
using CliWrap;
using CliWrap.Buffered;
using CSCG3DBAGPipeline.processing;
using Serilog;

namespace CSCG3DBAGPipeline;

public class Pipeline
{
    private AbstractDownloader _downloader;
    private CityJSONProcessor _cityJsonProcessor;
    private GLBProcessor _glbProcessor;
    private PipelineOptions _properties { get; init; }
    
    /// <summary>
    /// Constructor of our Pipeline, which downloads 3D BAG CityJSON files and converts them to Batched 3D Model files.
    /// </summary>
    /// <param name="properties">Object containing properties related to the pipeline.</param>
    public Pipeline(PipelineOptions properties)
    {
        this._properties = properties;
        this._downloader = new GzipDownloader(properties.FileWorkingDirectory);
        this._cityJsonProcessor = new CityJSONProcessor(
            properties.UpgradeFilterCjioScript,
            properties.ConvertToGLBScript,
            properties.FileWorkingDirectory);
        this._glbProcessor = new GLBProcessor(
            properties.ApplyDracoScript,
            properties.FileWorkingDirectory);
    }

    /// <summary>
    /// The main function of our pipeline. Currently hard coded to execute all tasks for all tiles selected.
    /// </summary>
    public async Task Process()
    {
        IList<int> skippedTileNumbers = new List<int>();
        for (int i = this._properties.StartTileNum; i < this._properties.LastTileNum; i++)
        {
            // Bijhouden welke files achteraf weer verwijderd dienen te worden
            IList<string> toBeDeletedFiles = new List<string>();
            
            // Basale setup
            int tile = this._properties.Tiles is null ? i : this._properties.Tiles.ElementAt(i);
            string cityJsonFile = $"{tile.ToString()}.json";
            string downloadPath = Path.Combine(_properties.DownloadDirectory, cityJsonFile);
            Console.WriteLine($"\n Now starting on {tile.ToString()}.");

            // Download 3D BAG CityJSON bestand
            string downloadUri = String.Format(this._properties.Base3DBAGUri, cityJsonFile);
            bool downloadRes = await this.Download(downloadUri, downloadPath);

            if (downloadRes == false) goto SkipAndLog;
            Console.WriteLine($"CityJSON Tile {tile.ToString()} has finished downloading from 3DBAG.");
            if (_properties.ClearDownload) toBeDeletedFiles.Add(downloadPath);

            // Gebruik cjio om bestand bij te werken naar CityJSON 1.1, filter onnodig detailniveau's en attributen
            var firstCjPath = downloadPath;
            var firstCjOutPath = Path.Combine(this._properties.FilteredDirectory,
                String.Format("filtered_{0}.json", tile.ToString()));
            bool firstCjRes = await this.ExecuteCommandAsyncAwait(
                this._cityJsonProcessor.FilterCityJSON,
                firstCjPath,
                firstCjOutPath);

            if (firstCjRes == false) goto SkipAndLog;
            Console.WriteLine($"CityJSON Tile {tile.ToString()} has been upgraded and filtered.");
            if (_properties.ClearFiltered) toBeDeletedFiles.Add(firstCjOutPath);

            // Gebruik CS-CityJSON-converter om features aan het maaiveld aan te passen (inclusief bounding box)
            var maaiveldOutPath = Path.Combine(this._properties.MaaiveldAdjustedFeaturesDirectory,
                String.Format("moved_{0}.json", tile.ToString()));
            bool maaiveldMoveRes = this.ExecuteStep(
                this._cityJsonProcessor.MoveMaaiveldToZero,
                firstCjOutPath,
                maaiveldOutPath);

            if (maaiveldMoveRes == false) goto SkipAndLog;
            Console.WriteLine($"CityJSON tile {tile.ToString()} features have been adjusted based on the Maaiveld.");
            if (_properties.ClearMaaiveldCorrected) toBeDeletedFiles.Add(maaiveldOutPath);

            // Gebruik cjio om CityJSON naar binaire glTF om te zetten
            var glbOutPath = Path.Combine(this._properties.GLBDirectory,
                String.Format("{0}.glb", tile.ToString()));
            bool glbConvertRes = await this.ExecuteCommandAsyncAwait(
                this._cityJsonProcessor.ConvertToGLB,
                maaiveldOutPath,
                glbOutPath);

            if (glbConvertRes == false) goto SkipAndLog;
            Console.WriteLine($"CityJSON tile {tile.ToString()} has been converted to a binary glTF file.");
            if (_properties.ClearGLB) toBeDeletedFiles.Add(glbOutPath);

            // Gebruik glTF Transform om binaire glTF te comprimeren (Draco)
            var glbDracoOutPath = Path.Combine(this._properties.DracoDirectory,
                String.Format("draco_{0}.glb", tile.ToString()));
            bool dracoCompressionRes = await this.ExecuteCommandAsyncAwait(
                this._glbProcessor.ApplyDracoCompression,
                glbOutPath,
                glbDracoOutPath);

            if (dracoCompressionRes == false) goto SkipAndLog;
            Console.WriteLine($"Binary glTF tile {tile.ToString()} has been compressed using Draco.");
            if (_properties.ClearDraco) toBeDeletedFiles.Add(glbDracoOutPath);

            // Gebruik B3DM Tile CS om ons glTF bestand in een B3DM te zetten
            var b3dmOutPath = Path.Combine(this._properties.B3dmDirectory,
                String.Format("{0}.b3dm", tile.ToString()));
            bool b3dmRes = this.ExecuteStep(this._glbProcessor.ToBatched3DModel,glbDracoOutPath, b3dmOutPath);

            if (b3dmRes == false) goto SkipAndLog;
            Console.WriteLine($"Draco compressed binary glTF tile {tile.ToString()} has been converted to a B3DM file.");
            // Ruim bestanden op
            this.DeleteUsedFiles(toBeDeletedFiles);
            Console.WriteLine($"Tile {tile.ToString()} has been successfully converted!");
            continue;

            SkipAndLog:
                // Niet vergeten op te ruimen na een skip
                skippedTileNumbers.Add(tile);
                this.DeleteUsedFiles(toBeDeletedFiles);
        }

        if (skippedTileNumbers.Any())
        {
            string skippedTileNumbersString = string.Join( ", ", skippedTileNumbers);
            Log.Warning($"Finished. The following tiles have not been processed for various reasons: {skippedTileNumbersString}");   
        }
    }

    private void DeleteUsedFiles(IList<string> toBeDeletedFiles)
    {
        FileHelpers.DeleteFiles(toBeDeletedFiles, workingDir: this._properties.FileWorkingDirectory);
    }

    /// <summary>
    /// Async Download function (GZip in this implementation!). Will download a file and store it.
    /// </summary>
    /// <param name="downloadUri">The download uri for the GZip file.</param>
    /// <param name="filename">The output file, relative to the working directory and including extension.</param>
    /// <returns>Boolean whether it was successful (if file was created).</returns>
    private async Task<bool> Download(string downloadUri, string filename)
    {
        try
        {
            await this._downloader.DownloadFile(downloadUri, filename);
            return FileHelpers.DoesFileExist(this._downloader.OutDir, filename);
        }
        catch (Exception e)
        {
            Log.Error(e, $"Encountered an error while downloading tile {filename}");
            return false;
        }

    }

    /// <summary>
    /// Execute a pipeline function which will create a new file.
    /// </summary>
    /// <param name="function">A pipeline function that creates a new file.</param>
    /// <param name="inFilePath">The input file, relative to the working directory and including extension.</param>
    /// <param name="outFilePath">The output file, relative to the working directory and including extension.</param>
    /// <returns>Boolean whether it was successful (if file was created).</returns>
    private bool ExecuteStep(
        Action<string, string> function,
        string inFilePath,
        string outFilePath)
    {
        try
        {
            function(inFilePath, outFilePath);
            return FileHelpers.DoesFileExist(this._properties.FileWorkingDirectory, outFilePath);
        }
        catch (Exception e)
        {
            Log.Error(e, $"Encountered an error while trying to process {inFilePath} using {function.Method.Name}");
            return false;
        }
    }
    
    /// <summary>
    /// Execute a async pipeline function which returns a CommandResult and creates a new file.. It will be awaited.
    /// </summary>
    /// <param name="function">An async pipeline function with a CommandResult as return type.</param>
    /// <param name="inFilePath">The input file, relative to the working directory and including extension.</param>
    /// <param name="outFilePath">The output file, relative to the working directory and including extension.</param>
    /// <returns>Boolean whether it was successful (if file was created).</returns>
    private async Task<bool> ExecuteCommandAsyncAwait(
        Func<string, string, Task<BufferedCommandResult>> function,
        string inFilePath,
        string outFilePath)
    {
        try
        {
            BufferedCommandResult res = await function(inFilePath, outFilePath);
            if (!string.IsNullOrEmpty(res.StandardError))
            {
                Log.Error($"Encountered an error while trying to process {inFilePath} using {function.Method.Name}. " +
                          $"The error occurred in a command-line, the following was reported: \n" +
                          $"Command: {res.StandardOutput} \n" +
                          $"Error: {res.StandardError}");
                return false;
            }
            return FileHelpers.DoesFileExist(this._properties.FileWorkingDirectory, outFilePath);
        }
        catch (Exception e)
        {
            Log.Error(e, $"Encountered an error while trying to process {inFilePath} using {function.Method.Name}");
            return false;
        }
    }
}