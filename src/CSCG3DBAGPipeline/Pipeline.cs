using CSCG3DBAGPipeline.processing;

namespace CSCG3DBAGPipeline;

public class Pipeline
{
    // Download
    // --Check if file has been downloaded
    // CityJSON/io first step
    // -- Check if file has been created
    // Move Maaiveld
    // -- Check if file has been created
    // CityJSON/io 2nd step
    // -- Check if file has been crated
    // GLB Draco Compression
    // -- Check if file has been created
    // B3DM Creation
    // -- Check if file has been created
    // Go to next item in list, deleted old files??
    
    private AbstractDownloader _downloader;
    private CityJSONProcessor _cityJsonProcessor;
    private GLBProcessor _glbProcessor;

    private PipelineProperties _properties { get; init; }
    
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

    public async Task Process()
    {
        foreach (int tile in this._properties.Tiles)
        {
            // Download 3D BAG CityJSON bestand
            string cityJsonFile = String.Format("{0}.json", tile.ToString());
            string downloadUri = String.Format(this._properties.Base3DBAGUri, cityJsonFile);
            bool downloadRes = await this.Download(downloadUri, cityJsonFile);
            
            if (downloadRes == false) continue;
            Console.WriteLine(String.Format("Done downloading tile {0}", tile.ToString()));
            
            // Gebruik cjio om bestand bij te werken naar CityJSON 1.1, filter onnodig detailniveau's en attributen
            var firstCjPath = Path.Combine(this._properties.CJ3DBAGDirectory, cityJsonFile);
            var firstCjOutPath = Path.Combine(this._properties.CJUpgradedFilteredDirectory, 
                String.Format("filtered_{0}.json", tile.ToString()));
            bool firstCjRes = await this.UpgradeFilterCJ(firstCjPath, firstCjOutPath);
            
            if (firstCjRes == false) continue;
            Console.WriteLine(String.Format("Done First CJ tile {0}", tile.ToString()));
            
            // Gebruik CS-CityJSON-converter om features aan het maaiveld aan te passen (inclusief bounding box)
            var maaiveldOutPath = Path.Combine(this._properties.MaaiveldCorrectCJDirectory,
                String.Format("moved_{0}.json", tile.ToString()));
            bool maaiveldMoveRes = this.ProcessMaaiveld(firstCjOutPath, maaiveldOutPath);
            
            if (maaiveldMoveRes == false) continue;
            Console.WriteLine(String.Format("Maaiveld CJ done for tile {0}", tile.ToString()));
            
            // Gebruik cjio om CityJSON naar binaire glTF om te zetten
            var glbOutPath = Path.Combine(this._properties.GLBDirectory,
                String.Format("{0}.glb", tile.ToString()));
            bool glbConvertRes = await this.ConvertToGLB(maaiveldOutPath, glbOutPath);
            
            if (glbConvertRes == false) continue;
            Console.WriteLine(String.Format("Tile {0} has been converted to binary glTF.", tile.ToString()));
            
            // Gebruik glTF Transform om binaire glTF te comprimeren (Draco)
            var glbDracoOutPath = Path.Combine(this._properties.DracoDirectory,
                String.Format("draco_{0}.glb", tile.ToString()));
            bool dracoCompressionRes = await this.CompressGLB(glbOutPath, glbDracoOutPath);
            
            if (dracoCompressionRes == false) continue;
            Console.WriteLine(String.Format("Binary glTF tile {0} has been compressed.", tile.ToString()));
            
            // Gebruik B3DM Tile CS om ons glTF bestand in een B3DM te zetten
            var b3dmOutPath = Path.Combine(this._properties.B3dmDirectory,
                String.Format("{0}.b3dm", tile.ToString()));
            bool b3dmRes = this.GenerateB3DM(glbDracoOutPath, b3dmOutPath);
            
            if (b3dmRes == false) continue;
            Console.WriteLine(String.Format("Tiles {0} has been converted to B3DM.", tile.ToString()));
        
        }
    }

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

    private async Task<bool> UpgradeFilterCJ(string inFilePath, string outFilePath)
    {
        try
        {
            var res = await this._cityJsonProcessor.FirstStep(inFilePath, outFilePath);
            Console.WriteLine(res);
            return FileHelpers.DoesFileExist(this._cityJsonProcessor._workingDirectory, outFilePath);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    private bool ProcessMaaiveld(string inFilePath, string outFilePath)
    {
        try
        {
            this._cityJsonProcessor.MoveMaaiveldToZero(inFilePath, outFilePath);
            return FileHelpers.DoesFileExist(this._cityJsonProcessor._workingDirectory, outFilePath);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
    
    private async Task<bool> ConvertToGLB(string inFilePath, string outFilePath)
    {
        try
        {
            await this._cityJsonProcessor.SecondStep(inFilePath, outFilePath);
            return FileHelpers.DoesFileExist(this._cityJsonProcessor._workingDirectory, outFilePath);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
    
    private async Task<bool> CompressGLB(string inFilePath, string outFilePath)
    {
        try
        {
            await this._glbProcessor.ApplyDracoCompression(inFilePath, outFilePath);
            return FileHelpers.DoesFileExist(this._glbProcessor._workingDirectory, outFilePath);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
    
    private bool GenerateB3DM(string inFilePath, string outFilePath)
    {
        try
        {
            this._glbProcessor.ToBatched3DModel(inFilePath, outFilePath);
            return FileHelpers.DoesFileExist(this._cityJsonProcessor._workingDirectory, outFilePath);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
}