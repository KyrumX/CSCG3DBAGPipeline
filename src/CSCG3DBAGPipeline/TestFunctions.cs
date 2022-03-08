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
            "fridget-pixelated_kaas.png",
            "fridget-pixelated.png",
        };

        foreach (var fileN in items)
        {
            try
            { 
                await downloader.DownloadFile(
                    downloadPath + fileN,
                    fileN);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        Console.WriteLine("Download done");
        
    }
}