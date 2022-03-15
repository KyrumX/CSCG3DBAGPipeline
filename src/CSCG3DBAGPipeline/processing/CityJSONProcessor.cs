using CliWrap;
using CliWrap.Buffered;
using CSCJConverter;

namespace CSCG3DBAGPipeline.processing;

public class CityJSONProcessor : Processor
{
    private readonly string _upgradeFilterFile;
    private readonly string _toGlbFile;

    /// <summary>
    /// Constructor for our CityJSON processor. This class processes functions related to CityJSON.
    /// </summary>
    /// <param name="upgradeFilterFile">Filename of the .bat file which uses cjio to filter/upgrade CityJSON.</param>
    /// <param name="toGlbFile">Filename of the .bat file which converts CityJSON to binary GLB.</param>
    /// <param name="workingDir">The path to our working directory, from here we will traverse.</param>
    public CityJSONProcessor(string upgradeFilterFile, string toGlbFile, string workingDir) : base(workingDir)
    {
        this._upgradeFilterFile = upgradeFilterFile;
        this._toGlbFile = toGlbFile;
    }

    /// <summary>
    /// Upgrades a 3DBAG CityJSON file to version 1.1, removes all detail levels except 2.2 and removes unnecessary
    /// attributes from our CityObjects. We use the upgradeFilter (bat) file and provide it 2 arguments:
    /// The first argument is the input file, the second argument is name the new file will have. 
    /// </summary>
    /// <param name="inFilePath">Filename (with extension) used as input, relative to the working directory.</param>
    /// <param name="outFilePath">Filename (with extension) of output file, relative to the working directory.</param>
    /// <returns>Task type CommandResult</returns>
    /// TODO: PATH EN FILE SPLITSEN?
    public async Task<BufferedCommandResult> FilterCityJSON(string inFilePath, string outFilePath)
    {
        Command cmd = this.CommandBuilder(this._upgradeFilterFile, new[] {inFilePath, outFilePath});
        BufferedCommandResult res = await cmd.ExecuteBufferedAsync();

        return res;
    }

    /// <summary>
    /// Converts a CityJSON file to binary glTF.
    /// </summary>
    /// <param name="inFilePath">Filename (with extension) used as input, relative to the working directory.</param>
    /// <param name="outFilePath">Filename (with extension) of output file, relative to the working directory.</param>
    /// <returns>Task type CommandResult</returns>
    public async Task<BufferedCommandResult> ConvertToGLB(string inFilePath, string outFilePath)
    {
        Command cmd = this.CommandBuilder(this._toGlbFile, new[] {inFilePath, outFilePath});
        BufferedCommandResult res = await cmd.ExecuteBufferedAsync();

        return res;
    }
    
    /// <summary>
    /// Takes a CityJSON 1.1 3D BAG file which has been filtered (LOD2.2 only!) and moves the Z-axis (height) to be
    /// 0 bound, meaning all features (buildings) will have their ground floor start at 0 meters. This is done by
    /// using the height of the maaiveld.
    /// </summary>
    /// <param name="inFilePath">Filename (with extension) used as input, relative to the working directory.</param>
    /// <param name="outFilePath">Filename (with extension) of output file, relative to the working directory.</param>
    public void MoveMaaiveldToZero(string inFilePath, string outFilePath)
    {
        // Bouw het path
        string jsonFilePath = Path.Combine(this._workingDirectory, inFilePath);
        string outJsonFilePath = Path.Combine(this._workingDirectory, outFilePath);

        // Lees JSON als string
        string jsonString = File.ReadAllText(jsonFilePath);
        
        // Deserialiseer JSON naar CityJSON model / maak converter object
        CityJSON cityJSONConverter = new CityJSON(jsonString, outJsonFilePath);
        
        // Translate maaiveld naar 0
        cityJSONConverter.TranslateHeightMaaiveld();
        
        // Update de het omvattende volume (geographical extent)
        cityJSONConverter.TransformGeographicalExtentZToZero();
        
        // Serialiseer terug naar JSON
        cityJSONConverter.Serialize();
    }
}