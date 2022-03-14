// See https://aka.ms/new-console-template for more information

using System.ComponentModel.DataAnnotations;
using CommandLine;
using CSCG3DBAGPipeline;

// Console.WriteLine("Hello, World!");
//
// PipelineProperties properties = new PipelineProperties(
//     new[] { 6227, 6228, 6229 },
//     @"E:\Hogeschool Rotterdam\Afstuderen CityGIS\Projects\CSCG3DBAGPipeline\cjio-upgrade-filter.bat",
//     @"E:\Hogeschool Rotterdam\Afstuderen CityGIS\Projects\CSCG3DBAGPipeline\cjio-glb.bat",
//     @"E:\Hogeschool Rotterdam\Afstuderen CityGIS\Projects\CSCG3DBAGPipeline\gltf-transform-draco-edgebreaker.bat",
//     @"E:\Hogeschool Rotterdam\Afstuderen CityGIS\Projects\CSCG3DBAGPipeline",
//     "https://data.3dbag.nl/cityjson/v210908_fd2cee53/3dbag_v210908_fd2cee53_{0}",
//     clearMaaiveld:true
// );
//
// PipelineProperties propertiesRange = new PipelineProperties(
//     1,
//     25,
//     @"E:\Hogeschool Rotterdam\Afstuderen CityGIS\Projects\CSCG3DBAGPipeline\cjio-upgrade-filter.bat",
//     @"E:\Hogeschool Rotterdam\Afstuderen CityGIS\Projects\CSCG3DBAGPipeline\cjio-glb.bat",
//     @"E:\Hogeschool Rotterdam\Afstuderen CityGIS\Projects\CSCG3DBAGPipeline\gltf-transform-draco-edgebreaker.bat",
//     @"E:\Hogeschool Rotterdam\Afstuderen CityGIS\Projects\CSCG3DBAGPipeline",
//     "https://data.3dbag.nl/cityjson/v210908_fd2cee53/3dbag_v210908_fd2cee53_{0}"
// );
//
// Pipeline pipeline = new Pipeline(properties); 
// await pipeline.Process();

// TODO: GEBRUIK PIPELINE PROPERTIES?

class Program
{
    [Verb("b3dm", HelpText = "Download from 3D BAG and convert to B3DM")]
    public class B3dmOptions
    {
        [Option('f', "files", SetName="List", Required = true, HelpText = "List of files.")]
        public IEnumerable<string> InputNumbers { get; set; }
        
        [Option('s', "startnum", SetName = "Range", Required = true)]
        public int StartNum { get; set; }
        [Option('e', "endnum", SetName = "Range", Required = true)]
        public int EndNum { get; set; }
        
        [Option("filterscript", Required = true)]
        public string CjioFilterScript { get; set; }
        [Option("glbscript", Required = true)]
        public string ConvertToGlbScript { get; set; }
        [Option("dracoscript", Required = true)]
        public string ApplyDracoScript { get; set; }
        [Option('u', "3dbaguri", Required = true)]
        public string DownloadUri { get; set; }
        [Option('d', "workingdirectory", Required = true)]
        public string WorkingDirectory { get; set; }
        [Option("downloaddir", Required = false)]
        public string DownloadDirectory { get; set; }
        [Option("filterdir", Required = false)]
        public string FilteredDirectory { get; set; }
        [Option("maaivelddir", Required = false)]
        public string MaaiveldDirectory { get; set; }
        [Option("glbdir", Required = false)]
        public string GlbDirectory { get; set; }
        [Option("dracodir", Required = false)]
        public string DracoDir { get; set; }
        [Option("b3dmdir", Required = false)]
        public string B3dmDir { get; set; }
    }

    [Verb("tileset", HelpText = "Generate a TileSet")]
    public class TilesetOptions
    {
        
    }
    static void Main(string[] args)
    {
        var result = Parser.Default.ParseArguments<B3dmOptions, TilesetOptions>(args)
            .WithParsed<B3dmOptions>(options => Console.WriteLine("!"))
            .WithParsed<TilesetOptions>(options => Console.WriteLine("222"))
            .WithNotParsed(errors => {});

    }

    static void RunAddAndReturnExitCode(B3dmOptions ops)
    {
        Console.WriteLine("Hey");
    }
}