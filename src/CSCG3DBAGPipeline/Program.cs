// See https://aka.ms/new-console-template for more information

using CommandLine;
using CSCG3DBAGPipeline;
using CSCG3DBAGPipeline.tileset;
using Serilog;

class Program
{
    public static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File("logs/log.txt",
                rollingInterval: RollingInterval.Hour,
                shared:true,
                retainedFileCountLimit: null,
                rollOnFileSizeLimit: true)
            .CreateLogger();

        await Parser.Default.ParseArguments<PipelineOptions, TilesetGeneratorOptions>(args)
            .MapResult(
                (PipelineOptions opts) => Batched3DPipeline(opts),
                (TilesetGeneratorOptions opts) => TilesetGenerator(opts),
                errs => Task.FromResult(0));

    }

    private static async Task Batched3DPipeline(PipelineOptions ops)
    {
        Pipeline pipeline = new Pipeline(ops);
        await pipeline.Process();
        Log.CloseAndFlush();
    }

    private static async Task TilesetGenerator(TilesetGeneratorOptions ops)
    {
        GridTilesetGenerator tilesetGenerator = new GridTilesetGenerator(ops);
        tilesetGenerator.AddTiles();
        tilesetGenerator.SerializeTileset();
        Log.CloseAndFlush();
    }
}
