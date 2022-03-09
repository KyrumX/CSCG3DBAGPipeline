using System.Text;
using CliWrap;

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

    public static async Task TestCommandFirstStep()
    {
        var processor = new CityJSONProcessor(
            @"E:\Hogeschool Rotterdam\Afstuderen CityGIS\Projects\CSCG3DBAGPipeline\cjio-upgrade-filter.bat",
            "",
            @"E:\Hogeschool Rotterdam\Afstuderen CityGIS\Projects\CSCG3DBAGPipeline"
        );

        var res = await processor.FirstStep("download/6229.json", "filtered/6229.json");

        Console.WriteLine("Heyyy");
    }

    public static async Task TestCommandMaaiveldAndGlb()
    {
        
        var processor = new CityJSONProcessor(
            @"E:\Hogeschool Rotterdam\Afstuderen CityGIS\Projects\CSCG3DBAGPipeline\cjio-upgrade-filter.bat",
            @"E:\Hogeschool Rotterdam\Afstuderen CityGIS\Projects\CSCG3DBAGPipeline\cjio-glb.bat",
            @"E:\Hogeschool Rotterdam\Afstuderen CityGIS\Projects\CSCG3DBAGPipeline"
        );

        var res = await processor.FirstStep("download/6229.json", "filtered/6229.json");

        processor.MoveMaaiveldToZero("filtered/6229.json", "moved/6229.json");

        var glbres = await processor.SecondStep("moved/6229.json", "glb/6229.glb");

    }
}