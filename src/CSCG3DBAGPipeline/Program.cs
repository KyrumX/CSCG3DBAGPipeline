// See https://aka.ms/new-console-template for more information
using CSCG3DBAGPipeline;

Console.WriteLine("Hello, World!");

PipelineProperties properties = new PipelineProperties(
    new[] { 6227, 6228, 6229 },
    @"E:\path\to\cjio-upgrade-filter.bat",
    @"E:\path\to\cjio-glb.bat",
    @"E:\path\to\gltf-transform-draco-edgebreaker.bat",
    @"E:\path\to\CSCG3DBAGPipeline",
    "https://data.3dbag.nl/cityjson/v210908_fd2cee53/3dbag_v210908_fd2cee53_{0}"
);

PipelineProperties propertiesRange = new PipelineProperties(
    6227,
    6229,
    @"E:\path\to\cjio-upgrade-filter.bat",
    @"E:\path\to\cjio-glb.bat",
    @"E:\path\to\gltf-transform-draco-edgebreaker.bat",
    @"E:\path\to\CSCG3DBAGPipeline",
    "https://data.3dbag.nl/cityjson/v210908_fd2cee53/3dbag_v210908_fd2cee53_{0}"
);

Pipeline pipeline = new Pipeline(propertiesRange); 
await pipeline.Process();