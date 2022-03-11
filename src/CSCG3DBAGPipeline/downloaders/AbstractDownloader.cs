namespace CSCG3DBAGPipeline;

public abstract class AbstractDownloader
{
    internal readonly string OutDir;

    protected AbstractDownloader(string outDir)
    {
        this.OutDir = outDir;
        Directory.CreateDirectory(this.OutDir);
    }
    
    public abstract Task DownloadFile(string fileUrl, string saveAs);
}