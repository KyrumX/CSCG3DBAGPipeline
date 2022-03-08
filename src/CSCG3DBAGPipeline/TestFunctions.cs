namespace CSCG3DBAGPipeline;

public static class TestFunctions
{
    public static async Task TestDownload()
    {
        var downloader = new Downloader(@"E:\Hogeschool Rotterdam\Afstuderen CityGIS\Projects\CSCG3DBAGPipeline\download");
        string downloadPath = "https://aaronbeetstra.me/static/images/projects/";
        String[] items = new[]
        {
            "ACELOGO_logo_los.png",
            "cf_mp_pix.png",
            "fridget-overview.jpg",
            "fridget-pixelated.png"
        };

        foreach (var fileN in items)
        {
            await downloader.DownloadFile(
                downloadPath + fileN,
                fileN);
        }

        Console.WriteLine("Download done");
        
    }
}