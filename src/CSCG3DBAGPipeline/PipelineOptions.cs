using CommandLine;

namespace CSCG3DBAGPipeline;

[Verb("b3dm", HelpText = "Download from 3D BAG and convert to B3DM")]
public class PipelineOptions
{
    // An Enumerable of type int of tiles. Cannot be used when 'Range' set options are being used (--endnum, --startnum).
    private readonly IEnumerable<int>? _tiles;
    [Option('f', "files", SetName="List", Required = true, HelpText = "List of tile numbers: 1 43 32")]
    public IEnumerable<int>? Tiles
    {
        get => _tiles;
        init => _tiles = value != null && value.Any() ? value : null;
    }
    
    // Number of the first tile. Cannot be used when 'List' set option is in use (--files).
    [Option('s', "startnum", SetName = "Range", Required = true, HelpText = "Starting tile number.")]
    public int StartTileNum { get; init; }
    // Number of the last tile, is inclusive. Cannot be used when 'List' set option is in use (--files).
    private readonly int _lastTileNum;
    [Option('e', "endnum", SetName = "Range", Required = true, HelpText = "Final tile number, is inclusive.")]
    public int LastTileNum
    {
        get => _lastTileNum;
        init => _lastTileNum = value + 1;
    }

    // Path to our file working directory. Here all our other folders (b3dm, draco, downloads) will be placed.
    private readonly string _workingDirectory;
    [Option('d', "workingdir", Required = false, Default = "", HelpText = "The working directory. Empty will result in the current directory being used.")]
    public string FileWorkingDirectory
    {
        get => _workingDirectory;
        init => _workingDirectory = string.IsNullOrEmpty(value) ? System.AppDomain.CurrentDomain.BaseDirectory : value;
    }
    // Path to the script which will use CityJSON/io to upgrade and filter our CityJSON file.
    [Option("filterscript", Required = true, HelpText = "Path to the CityJSON/io script which will filter CityJSON.")]
    public string UpgradeFilterCjioScript { get; init; }
    // Path to the script which will use CityJSON/io to convert our CityJSON to binary glTF.
    [Option("glbscript", Required = true, HelpText = "Path to the CityJSON/io script which will convert CityJSON to binary glTF.")]
    public string ConvertToGLBScript { get; init; }
    // Path to the script which will use glTF Transfrom to compress our binary glTF using Draco.
    [Option("dracoscript", Required = true, HelpText = "Path to the gltf-transform script which will compress binary glTF.")]
    public string ApplyDracoScript { get; init; }
    // Base uri for our 3D BAG CityJSON files, use a '{0}' for the place where the tile number and extension would have
    // been! Example: https://data.3dbag.nl/cityjson/v210908_fd2cee53/3dbag_v210908_fd2cee53_3015.json becomes
    // https://data.3dbag.nl/cityjson/v210908_fd2cee53/3dbag_v210908_fd2cee53_{0}
    [Option('u', "3dbaguri", Required = true, HelpText = "Base uri for CityJSON tiles from 3D BAG. Use `{0}` where the tile number and extension should be, example: `https://data.3dbag.nl/cityjson/v210908_fd2cee53/3dbag_v210908_fd2cee53_{0}`")]
    public string Base3DBAGUri { get; init; }

    // Directory, relative to our file working directory, where CityJSON will be downloaded to.
    private readonly string _downloadDirectory;
    [Option("downloaddir", Required = false, Default = "files/downloads", HelpText = "Directory where downloads will be stored, relative to the working directory.")]
    public string DownloadDirectory
    {
        get => _downloadDirectory;
        init
        {
            Console.WriteLine(Path.Combine(FileWorkingDirectory, value));
            Directory.CreateDirectory(Path.Combine(FileWorkingDirectory, value));
            _downloadDirectory = value;
        }
    }

    // Directory, relative to our file working directory, where (upgraded &) filtered CityJSON will be stored.
    private readonly string _filteredDirectory;
    [Option("filterdir", Required = false, Default = "files/filtered", HelpText = "Directory where filtered/upgraded CityJSON will be stored, relative to the working directory.")]
    public string FilteredDirectory
    {
        get => _filteredDirectory;
        init
        {
            Directory.CreateDirectory(Path.Combine(FileWorkingDirectory, value));
            _filteredDirectory = value;
        }
    }

    // Directory, relative to our file working directory, where CityJSON with features that have been adjusted based
    // on the maaiveld value will be stored.
    private readonly string _maaiveldAdjustedFeaturesDirectory;
    [Option("maaivelddir", Required = false, Default = "files/maaiveld", HelpText = "Directory where CityJSON with features that have been moved based on the maaiveld will be stored, relative to the working directory.")]
    public string MaaiveldAdjustedFeaturesDirectory {
        get => _maaiveldAdjustedFeaturesDirectory;
        init
        {
            Directory.CreateDirectory(Path.Combine(FileWorkingDirectory, value));
            _maaiveldAdjustedFeaturesDirectory = value;
        } }

    // Directory, relative to our file working directory, where binary glTF (GLB) will be stored.
    private readonly string _glbDirectory;
    [Option("glbdir", Required = false, Default = "files/glb", HelpText = "Directory where binary glTF will be stored, relative to the working directory.")]
    public string GLBDirectory
    {
        get => _glbDirectory;
        init
        {
            Directory.CreateDirectory(Path.Combine(FileWorkingDirectory, value));
            _glbDirectory = value;
        }
    }

    // Directory, relative to our file working directory, where Draco compressed GLB will be stored.
    private readonly string _dracoDirectory;
    [Option("dracodir", Required = false, Default = "files/draco", HelpText = "Directory where Draco compressed GLB will be stored, relative to the working directory.")]
    public string DracoDirectory
    {
        get => _dracoDirectory;
        init
        {
            Directory.CreateDirectory(Path.Combine(FileWorkingDirectory, value));
            _dracoDirectory = value;
        }
    }

    // Directory, relative to our file working directory, where Bathced 3D Models will be stored.
    private readonly string _b3dmDirectory;
    [Option("b3dmdir", Required = false, Default = "files/b3dm", HelpText = "Directory where Batched 3D Models will be stored, relative to the working directory.")]
    public string B3dmDirectory
    {
        get => _b3dmDirectory;
        init
        {
            Directory.CreateDirectory(Path.Combine(FileWorkingDirectory, value));
            _b3dmDirectory = value;
        }
    }
    
    // Whether or not a download (from 3D BAG) should be deleted after having been processed.
    [Option("cleardownload", Required = false, Default = true, HelpText = "Whether or not a download should be deleted after having been processed.")]
    public bool ClearDownload { get; init; }
    // Whether or not a filtered CityJSON file should be deleted after having been processed.
    [Option("clearfiltered", Required = false, Default = true, HelpText = "Whether or not a filtered CityJSON file should be deleted after having been processed.")]
    public bool ClearFiltered { get; init; }
    // Whether or not the maaiveld adjusted CityJSON file should be deleted after having been processed.
    [Option("clearmaaiveld", Required = false, Default = false, HelpText = "Whether or not a CityJSON with features that have been adjusted based on the maaiveld should be deleted after having been processed.")]
    public bool ClearMaaiveldCorrected { get; init; }
    // Whether or not a binairy gLTF file should be deleted after having been processed.
    [Option("clearglb", Required = false, Default = true, HelpText = "Whether or not a binary glTF file should be deleted after having been processed.")]
    public bool ClearGLB { get; init; }
    // Whether or not a Draco compressed binary glTF file should be deleted after having been processed.
    [Option("cleardraco", Required = false, Default = true, HelpText = "Whether or not a Draco compressed GLB should be deleted after having been processed.")]
    public bool ClearDraco { get; init; }
}