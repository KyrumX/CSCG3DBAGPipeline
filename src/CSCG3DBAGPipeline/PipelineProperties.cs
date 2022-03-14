using CommandLine;

namespace CSCG3DBAGPipeline;

[Verb("b3dm", HelpText = "Download from 3D BAG and convert to B3DM")]
public class PipelineProperties
{
    private readonly IEnumerable<int>? _tiles;
    [Option('f', "files", SetName="List", Required = true, HelpText = "List of files.")]
    public IEnumerable<int>? Tiles
    {
        get => _tiles;
        init => _tiles = value != null && value.Any() ? value : null;
    }
    
    [Option('s', "startnum", SetName = "Range", Required = true)]
    public int StartTileNum { get; init; }
    private readonly int _lastTileNum;

    [Option('e', "endnum", SetName = "Range", Required = true)]
    public int LastTileNum
    {
        get => _lastTileNum;
        init => _lastTileNum = value + 1;
    }
    
    [Option('d', "workingdir", Required = true)]
    public string FileWorkingDirectory { get; init; }
    [Option("filterscript", Required = true)]
    public string UpgradeFilterCjioScript { get; init; }
    [Option("glbscript", Required = true)]
    public string ConvertToGLBScript { get; init; }
    [Option("dracoscript", Required = true)]
    public string ApplyDracoScript { get; init; }
    [Option('u', "3dbaguri", Required = true)]
    public string Base3DBAGUri { get; init; }

    // Directory, relative to our file working directory, where CityJSON will be downloaded to.
    private readonly string _downloadDirectory;
    [Option("downloaddir", Required = false, Default = "files/downloads")]
    public string DownloadDirectory
    {
        get => _downloadDirectory;
        init
        {
            Directory.CreateDirectory(Path.Combine(FileWorkingDirectory, value));
            _downloadDirectory = value;
        }
    }

    // Directory, relative to our file working directory, where (upgraded &) filtered CityJSON will be stored.
    private readonly string _filteredDirectory;
    [Option("filterdir", Required = false, Default = "files/upgraded")]
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
    [Option("maaivelddir", Required = false, Default = "files/maaiveld")]
    public string MaaiveldAdjustedFeaturesDirectory {
        get => _maaiveldAdjustedFeaturesDirectory;
        init
        {
            Directory.CreateDirectory(Path.Combine(FileWorkingDirectory, value));
            _maaiveldAdjustedFeaturesDirectory = value;
        } }

    // Directory, relative to our file working directory, where binary glTF (GLB) will be stored.
    private readonly string _glbDirectory;
    [Option("glbdir", Required = false, Default = "files/glb")]
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
    [Option("dracodir", Required = false, Default = "files/draco")]
    public string DracoDirectory
    {
        get => _dracoDirectory;
        init
        {
            Directory.CreateDirectory(Path.Combine(FileWorkingDirectory, value));
            _dracoDirectory = value;
        }
    }

    private readonly string _b3dmDirectory;

    // Directory, relative to our file working directory, where Bathced 3D Models will be stored.
    [Option("b3dmdir", Required = false, Default = "files/b3dm")]
    public string B3dmDirectory
    {
        get => _b3dmDirectory;
        init
        {
            Directory.CreateDirectory(Path.Combine(FileWorkingDirectory, value));
            _b3dmDirectory = value;
        }
    }
    
    [Option("cleardownload", Required = false, Default = true)]
    public bool ClearDownload { get; init; }
    [Option("clearfiltered", Required = false, Default = true)]
    public bool ClearFiltered { get; init; }
    [Option("clearmaaiveld", Required = false, Default = false)]
    public bool ClearMaaiveldCorrected { get; init; }
    [Option("clearglb", Required = false, Default = true)]
    public bool ClearGLB { get; init; }
    [Option("cleardraco", Required = false, Default = true)]
    public bool ClearDraco { get; init; }

    /// <summary>
    /// Constructor for a a fixed list of tiles.
    /// </summary>
    /// <param name="tiles">An Enumerable of type int of tiles.</param>
    /// <param name="upgradeFilterCjioScript">Path to the script which will use CityJSON/io to upgrade and filter our CityJSON file.</param>
    /// <param name="convertToGlbScript">Path to the script which will use CityJSON/io to convert our CityJSON to binary glTF.</param>
    /// <param name="applyDracoScript">Path to the script which will use glTF Transfrom to compress our binary glTF using Draco.</param>
    /// <param name="fileWorkingDirectory">Path to our file working directory.</param>
    /// <param name="base3DbagUri">Base uri for 3D BAG downloads. Format: https://data.3dbag.nl/cityjson/v210908_fd2cee53/3dbag_v210908_fd2cee53_ where the number and extension have been removed!</param>
    /// <param name="cj3DbagDirectory">Directory, relative to our file working directory, where CityJSON will be downloaded to.</param>
    /// <param name="cjUpgradedFilteredDirectory">Directory, relative to our file working directory, where upgraded/filtered CityJSON will be stored.</param>
    /// <param name="maaiveldCorrectCjDirectory">Directory, relative to our file working directory, where CityJSON which has been adjusted will be stored.</param>
    /// <param name="glbDirectory">Directory, relative to our file working directory, where binary glTF (GLB) will be stored.</param>
    /// <param name="dracoDirectory">Directory, relative to our file working directory, where Draco compressed GLB will be stored.</param>
    /// <param name="b3dmDirectory">Directory, relative to our file working directory, where B3DM will be stored.</param>
    /// <param name="clearDownloads">Boolean, whether or not a download (from 3D BAG) should be deleted after having been processed.</param>
    /// <param name="clearFiltered">Boolean, whether or not a filtered CityJSON file should be deleted after having been processed.</param>
    /// <param name="clearMaaiveld">Boolean, whether or not the maaiveld adjusted CityJSON file should be deleted after having been processed.</param>
    /// <param name="clearGlb">Boolean, whether or not a binairy gLTF file should be deleted after having been processed.</param>
    /// <param name="clearDraco">Boolean, whether or not a Draco compressed binary glTF file should be deleted after having been processed.</param>
    // public PipelineProperties(
    //     IEnumerable<int> tiles,
    //     string upgradeFilterCjioScript,
    //     string convertToGlbScript,
    //     string applyDracoScript,
    //     string fileWorkingDirectory,
    //     string base3DbagUri,
    //     string cj3DbagDirectory = "files/downloads",
    //     string cjUpgradedFilteredDirectory = "files/upgraded",
    //     string maaiveldCorrectCjDirectory = "files/maaiveld",
    //     string glbDirectory = "files/glb", 
    //     string dracoDirectory = "files/draco",
    //     string b3dmDirectory = "files/b3dm", 
    //     bool clearDownloads = true,
    //     bool clearFiltered = true,
    //     bool clearMaaiveld = false,
    //     bool clearGlb = true,
    //     bool clearDraco = true
    // )
    // {
    //     this.Tiles = tiles;
    //     this.UpgradeFilterCjioScript = upgradeFilterCjioScript;
    //     this.ConvertToGLBScript = convertToGlbScript;
    //     this.ApplyDracoScript = applyDracoScript;
    //     this.Base3DBAGUri = base3DbagUri;
    //     this.FileWorkingDirectory = fileWorkingDirectory;
    //     this.CJ3DBAGDirectory = cj3DbagDirectory;
    //     this.CJUpgradedFilteredDirectory = cjUpgradedFilteredDirectory;
    //     this.MaaiveldCorrectCJDirectory = maaiveldCorrectCjDirectory;
    //     this.GLBDirectory = glbDirectory;
    //     this.DracoDirectory = dracoDirectory;
    //     this.B3dmDirectory = b3dmDirectory;
    //     this.ClearDownload = clearDownloads;
    //     this.ClearFiltered = clearFiltered;
    //     this.ClearMaaiveldCorrected = clearMaaiveld;
    //     this.ClearGLB = clearGlb;
    //     this.ClearDraco = clearDraco;
    //     this.StartTileNum = 0;
    //     this.LastTileNum = this.Tiles.Count();
    // }
    //
    // /// <summary>
    // /// Constructor for a range of tiles, ergo start at tile 10 end at tile 20 (inclusive!).
    // /// </summary>
    // /// <param name="startTileNum">Number of the first tile.</param>
    // /// <param name="lastTileNum">Number of the last tile, is inclusive!</param>
    // /// <param name="upgradeFilterCjioScript">Path to the script which will use CityJSON/io to upgrade and filter our CityJSON file.</param>
    // /// <param name="convertToGlbScript">Path to the script which will use CityJSON/io to convert our CityJSON to binary glTF.</param>
    // /// <param name="applyDracoScript">Path to the script which will use glTF Transfrom to compress our binary glTF using Draco.</param>
    // /// <param name="fileWorkingDirectory">Path to our file working directory.</param>
    // /// <param name="base3DbagUri">Base uri for 3D BAG downloads. Format: https://data.3dbag.nl/cityjson/v210908_fd2cee53/3dbag_v210908_fd2cee53_ where the number and extension have been removed!</param>
    // /// <param name="cj3DbagDirectory">Directory, relative to our file working directory, where CityJSON will be downloaded to.</param>
    // /// <param name="cjUpgradedFilteredDirectory">Directory, relative to our file working directory, where upgraded/filtered CityJSON will be stored.</param>
    // /// <param name="maaiveldCorrectCjDirectory">Directory, relative to our file working directory, where CityJSON which has been adjusted will be stored.</param>
    // /// <param name="glbDirectory">Directory, relative to our file working directory, where binary glTF (GLB) will be stored.</param>
    // /// <param name="dracoDirectory">Directory, relative to our file working directory, where Draco compressed GLB will be stored.</param>
    // /// <param name="b3dmDirectory">Directory, relative to our file working directory, where B3DM will be stored.</param>
    // /// <param name="clearDownloads">Boolean, whether or not a download (from 3D BAG) should be deleted after having been processed.</param>
    // /// <param name="clearFiltered">Boolean, whether or not a filtered CityJSON file should be deleted after having been processed.</param>
    // /// <param name="clearMaaiveld">Boolean, whether or not the maaiveld adjusted CityJSON file should be deleted after having been processed.</param>
    // /// <param name="clearGlb">Boolean, whether or not a binairy gLTF file should be deleted after having been processed.</param>
    // /// <param name="clearDraco">Boolean, whether or not a Draco compressed binary glTF file should be deleted after having been processed.</param>
    // public PipelineProperties(
    //     int startTileNum,
    //     int lastTileNum,
    //     string upgradeFilterCjioScript,
    //     string convertToGlbScript,
    //     string applyDracoScript,
    //     string fileWorkingDirectory,
    //     string base3DbagUri,
    //     string cj3DbagDirectory = "files/downloads",
    //     string cjUpgradedFilteredDirectory = "files/upgraded",
    //     string maaiveldCorrectCjDirectory = "files/maaiveld",
    //     string glbDirectory = "files/glb", 
    //     string dracoDirectory = "files/draco",
    //     string b3dmDirectory = "files/b3dm",
    //     bool clearDownloads = true,
    //     bool clearFiltered = true,
    //     bool clearMaaiveld = false,
    //     bool clearGlb = true,
    //     bool clearDraco = true
    // )
    // {
    //     this.StartTileNum = startTileNum;
    //     // We willen dat deze tegel inclusief is
    //     this.LastTileNum = lastTileNum + 1;
    //     this.UpgradeFilterCjioScript = upgradeFilterCjioScript;
    //     this.ConvertToGLBScript = convertToGlbScript;
    //     this.ApplyDracoScript = applyDracoScript;
    //     this.Base3DBAGUri = base3DbagUri;
    //     this.FileWorkingDirectory = fileWorkingDirectory;
    //     this.CJ3DBAGDirectory = cj3DbagDirectory;
    //     this.CJUpgradedFilteredDirectory = cjUpgradedFilteredDirectory;
    //     this.MaaiveldCorrectCJDirectory = maaiveldCorrectCjDirectory;
    //     this.GLBDirectory = glbDirectory;
    //     this.DracoDirectory = dracoDirectory;
    //     this.B3dmDirectory = b3dmDirectory;
    //     this.ClearDownload = clearDownloads;
    //     this.ClearFiltered = clearFiltered;
    //     this.ClearMaaiveldCorrected = clearMaaiveld;
    //     this.ClearGLB = clearGlb;
    //     this.ClearDraco = clearDraco;
    // }
}