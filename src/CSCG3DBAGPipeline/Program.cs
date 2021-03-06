// See https://aka.ms/new-console-template for more information

using CommandLine;
using CSCG3DBAGPipeline.tileset;
using Serilog;

namespace CSCG3DBAGPipeline;

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

    /// <summary>
    /// Method for generating B3DM from 3D BAG CityJSON.
    /// </summary>
    /// <param name="ops">Options object: PipelineOptions</param>
    private static async Task Batched3DPipeline(PipelineOptions ops)
    {
        Pipeline pipeline = new Pipeline(ops);
        await pipeline.Process();
        Log.CloseAndFlush();
    }

    /// <summary>
    /// Method for generating tileset.
    /// </summary>
    /// <param name="ops">Options object: TilesetGeneratorOptions</param>
    private static async Task TilesetGenerator(TilesetGeneratorOptions ops)
    {
        TilesetGenerator tilesetGenerator = new TilesetGenerator(ops);
        tilesetGenerator.AddTiles();
        tilesetGenerator.SerializeTileset();
        Log.CloseAndFlush();
    }
}