namespace CSCG3DBAGPipeline;

public class PipelineProperties
{
    public IEnumerable<int>? Tiles { get; init; }
    public int StartTileNum { get; init; }
    public int LastTileNum { get; init; }
    public string FileWorkingDirectory { get; init; }
    public string UpgradeFilterCjioScript { get; init; }
    public string ConvertToGLBScript { get; init; }
    public string ApplyDracoScript { get; init; }
    public string Base3DBAGUri { get; init; }
    public string CJ3DBAGDirectory { get; init; }
    public string CJUpgradedFilteredDirectory { get; init; }
    public string MaaiveldCorrectCJDirectory { get; init; }
    public string GLBDirectory { get; init; }
    public string DracoDirectory { get; init; }
    public string B3dmDirectory { get; init; }
    public bool ClearDownload { get; init; }
    public bool ClearFiltered { get; init; }
    public bool ClearMaaiveldCorrected { get; init; }
    public bool ClearGLB { get; init; }
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
    public PipelineProperties(
        IEnumerable<int> tiles,
        string upgradeFilterCjioScript,
        string convertToGlbScript,
        string applyDracoScript,
        string fileWorkingDirectory,
        string base3DbagUri,
        string cj3DbagDirectory = "files/downloads",
        string cjUpgradedFilteredDirectory = "files/upgraded",
        string maaiveldCorrectCjDirectory = "files/maaiveld",
        string glbDirectory = "files/glb", 
        string dracoDirectory = "files/draco",
        string b3dmDirectory = "files/b3dm", 
        bool clearDownloads = true,
        bool clearFiltered = true,
        bool clearMaaiveld = false,
        bool clearGlb = true,
        bool clearDraco = true
    )
    {
        this.Tiles = tiles;
        this.UpgradeFilterCjioScript = upgradeFilterCjioScript;
        this.ConvertToGLBScript = convertToGlbScript;
        this.ApplyDracoScript = applyDracoScript;
        this.Base3DBAGUri = base3DbagUri;
        this.FileWorkingDirectory = fileWorkingDirectory;
        this.CJ3DBAGDirectory = cj3DbagDirectory;
        this.CJUpgradedFilteredDirectory = cjUpgradedFilteredDirectory;
        this.MaaiveldCorrectCJDirectory = maaiveldCorrectCjDirectory;
        this.GLBDirectory = glbDirectory;
        this.DracoDirectory = dracoDirectory;
        this.B3dmDirectory = b3dmDirectory;
        this.ClearDownload = clearDownloads;
        this.ClearFiltered = clearFiltered;
        this.ClearMaaiveldCorrected = clearMaaiveld;
        this.ClearGLB = clearGlb;
        this.ClearDraco = clearDraco;
        this.StartTileNum = 0;
        this.LastTileNum = this.Tiles.Count();
    }

    /// <summary>
    /// Constructor for a range of tiles, ergo start at tile 10 end at tile 20 (inclusive!).
    /// </summary>
    /// <param name="startTileNum">Number of the first tile.</param>
    /// <param name="lastTileNum">Number of the last tile, is inclusive!</param>
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
    public PipelineProperties(
        int startTileNum,
        int lastTileNum,
        string upgradeFilterCjioScript,
        string convertToGlbScript,
        string applyDracoScript,
        string fileWorkingDirectory,
        string base3DbagUri,
        string cj3DbagDirectory = "files/downloads",
        string cjUpgradedFilteredDirectory = "files/upgraded",
        string maaiveldCorrectCjDirectory = "files/maaiveld",
        string glbDirectory = "files/glb", 
        string dracoDirectory = "files/draco",
        string b3dmDirectory = "files/b3dm",
        bool clearDownloads = true,
        bool clearFiltered = true,
        bool clearMaaiveld = false,
        bool clearGlb = true,
        bool clearDraco = true
    )
    {
        this.StartTileNum = startTileNum;
        // We willen dat deze tegel inclusief is
        this.LastTileNum = lastTileNum + 1;
        this.UpgradeFilterCjioScript = upgradeFilterCjioScript;
        this.ConvertToGLBScript = convertToGlbScript;
        this.ApplyDracoScript = applyDracoScript;
        this.Base3DBAGUri = base3DbagUri;
        this.FileWorkingDirectory = fileWorkingDirectory;
        this.CJ3DBAGDirectory = cj3DbagDirectory;
        this.CJUpgradedFilteredDirectory = cjUpgradedFilteredDirectory;
        this.MaaiveldCorrectCJDirectory = maaiveldCorrectCjDirectory;
        this.GLBDirectory = glbDirectory;
        this.DracoDirectory = dracoDirectory;
        this.B3dmDirectory = b3dmDirectory;
        this.ClearDownload = clearDownloads;
        this.ClearFiltered = clearFiltered;
        this.ClearMaaiveldCorrected = clearMaaiveld;
        this.ClearGLB = clearGlb;
        this.ClearDraco = clearDraco;
    }
}