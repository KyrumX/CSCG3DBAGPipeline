// See https://aka.ms/new-console-template for more information

using System.Net;
using System.Text;
using CliWrap;
using CSCG3DBAGPipeline;

Console.WriteLine("Hello, World!");

PipelineProperties properties = new PipelineProperties(
    new[] { 6227, 6228, 6229 },
    @"E:\Hogeschool Rotterdam\Afstuderen CityGIS\Projects\CSCG3DBAGPipeline\cjio-upgrade-filter.bat",
    @"E:\Hogeschool Rotterdam\Afstuderen CityGIS\Projects\CSCG3DBAGPipeline\cjio-glb.bat",
    @"E:\Hogeschool Rotterdam\Afstuderen CityGIS\Projects\CSCG3DBAGPipeline\gltf-transform-draco-edgebreaker.bat",
    @"E:\Hogeschool Rotterdam\Afstuderen CityGIS\Projects\CSCG3DBAGPipeline\files",
    "https://data.3dbag.nl/cityjson/v210908_fd2cee53/3dbag_v210908_fd2cee53_{0}"
);

Pipeline pipeline = new Pipeline(properties); 
await pipeline.Process();