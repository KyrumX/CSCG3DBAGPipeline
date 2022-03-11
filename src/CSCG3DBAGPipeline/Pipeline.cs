using CliWrap;
using CSCG3DBAGPipeline.processing;

namespace CSCG3DBAGPipeline;

public class Pipeline
{
    private AbstractDownloader _downloader;
    private CityJSONProcessor _cityJsonProcessor;
    private GLBProcessor _glbProcessor;
    private PipelineProperties _properties { get; init; }
    
    /// <summary>
    /// Constructor of our Pipeline, which downloads 3D BAG CityJSON files and converts them to Batched 3D Model files.
    /// </summary>
    /// <param name="properties">Object containing properties related to the pipeline.</param>
    public Pipeline(PipelineProperties properties)
    {
        this._properties = properties;
        this._downloader = new GzipDownloader(Path.Combine(properties.FileWorkingDirectory, properties.CJ3DBAGDirectory));
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
        for (int i = this._properties.StartTileNum; i < this._properties.LastTileNum; i++)
        {
            int tile = this._properties.Tiles is null ? i : this._properties.Tiles.ElementAt(i);
            string cityJsonFile = $"{tile.ToString()}.json";
            Console.WriteLine($"\n Now starting on {tile.ToString()}.");
            
            // Download 3D BAG CityJSON bestand
            string downloadUri = String.Format(this._properties.Base3DBAGUri, cityJsonFile);
            bool downloadRes = await this.Download(downloadUri, cityJsonFile);

            if (downloadRes == false) continue;
            Console.WriteLine($"CityJSON Tile {tile.ToString()} has finished downloading from 3DBAG.");

            // Gebruik cjio om bestand bij te werken naar CityJSON 1.1, filter onnodig detailniveau's en attributen
            var firstCjPath = Path.Combine(this._properties.CJ3DBAGDirectory, cityJsonFile);
            var firstCjOutPath = Path.Combine(this._properties.CJUpgradedFilteredDirectory,
                String.Format("filtered_{0}.json", tile.ToString()));
            bool firstCjRes = await this.ExecuteCommandAsyncAwait(
                this._cityJsonProcessor.FirstStep,
                firstCjPath,
                firstCjOutPath);

            if (firstCjRes == false) continue;
            Console.WriteLine($"CityJSON Tile {tile.ToString()} has been upgraded and filtered.");

            // Gebruik CS-CityJSON-converter om features aan het maaiveld aan te passen (inclusief bounding box)
            var maaiveldOutPath = Path.Combine(this._properties.MaaiveldCorrectCJDirectory,
                String.Format("moved_{0}.json", tile.ToString()));
            bool maaiveldMoveRes = this.ExecuteStep(
                this._cityJsonProcessor.MoveMaaiveldToZero,
                firstCjOutPath,
                maaiveldOutPath);

            if (maaiveldMoveRes == false) continue;
            Console.WriteLine($"CityJSON tile {tile.ToString()} features have adjusted based on the Maaiveld.");

            // Gebruik cjio om CityJSON naar binaire glTF om te zetten
            var glbOutPath = Path.Combine(this._properties.GLBDirectory,
                String.Format("{0}.glb", tile.ToString()));
            bool glbConvertRes = await this.ExecuteCommandAsyncAwait(
                this._cityJsonProcessor.SecondStep,
                maaiveldOutPath,
                glbOutPath);

            if (glbConvertRes == false) continue;
            Console.WriteLine($"CityJSON tile {tile.ToString()} has been converted to a binary glTF file.");

            // Gebruik glTF Transform om binaire glTF te comprimeren (Draco)
            var glbDracoOutPath = Path.Combine(this._properties.DracoDirectory,
                String.Format("draco_{0}.glb", tile.ToString()));
            bool dracoCompressionRes = await this.ExecuteCommandAsyncAwait(
                this._glbProcessor.ApplyDracoCompression,
                glbOutPath,
                glbDracoOutPath);

            if (dracoCompressionRes == false) continue;
            Console.WriteLine($"Binary glTF tile {tile.ToString()} has been compressed using Draco.");

            // Gebruik B3DM Tile CS om ons glTF bestand in een B3DM te zetten
            var b3dmOutPath = Path.Combine(this._properties.B3dmDirectory,
                String.Format("{0}.b3dm", tile.ToString()));
            bool b3dmRes = this.ExecuteStep(this._glbProcessor.ToBatched3DModel,glbDracoOutPath, b3dmOutPath);

            if (b3dmRes == false) continue;
            Console.WriteLine($"Draco compressed binary glTF tile {tile.ToString()} has been converted to a B3DM file.");
        }
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
            Console.WriteLine(e);
            // TODO: log
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
            Console.WriteLine(e);
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
        Func<string, string, Task<CommandResult>> function,
        string inFilePath,
        string outFilePath)
    {
        try
        {
            CommandResult res = await function(inFilePath, outFilePath);
            return FileHelpers.DoesFileExist(this._properties.FileWorkingDirectory, outFilePath);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    // LogFailure en DeleteFile besprekenen voor verdere implementatie
    private void LogFailure()
    {
        
    }

    private void DeleteFile()
    {
        
    }
}