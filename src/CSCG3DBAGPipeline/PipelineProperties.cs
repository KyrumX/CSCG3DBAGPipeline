namespace CSCG3DBAGPipeline;

public class PipelineProperties
{
    public IEnumerable<int> Tiles { get; init; }
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

    public PipelineProperties(
        IEnumerable<int> tiles,
        string upgradeFilterCjioScript,
        string convertToGlbScript,
        string applyDracoScript,
        string fileWorkingDirectory,
        string base3DbagUri,
        string cj3DbagDirectory = "downloads",
        string cjUpgradedFilteredDirectory = "upgraded",
        string maaiveldCorrectCjDirectory = "maaiveld",
        string glbDirectory = "glb", 
        string dracoDirectory = "draco",
        string b3dmDirectory = "b3dm"
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
    }
}