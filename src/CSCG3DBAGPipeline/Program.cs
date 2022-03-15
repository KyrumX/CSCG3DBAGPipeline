// See https://aka.ms/new-console-template for more information

using CommandLine;
using CSCG3DBAGPipeline;
class Program
{
    [Verb("tileset", HelpText = "Generate a TileSet")]
    public class TilesetOptions
    {
        
    }
    async static Task Main(string[] args)
    {
        // var result = Parser.Default.ParseArguments<PipelineProperties, TilesetOptions>(args)
        //     .WithParsed<PipelineProperties>(options => Batched3DPipeline(options))
        //     .WithParsed<TilesetOptions>(options => Console.WriteLine("222"))
        //     .WithNotParsed(errors => {});
        var result = await Parser.Default.ParseArguments<PipelineOptions, TilesetOptions>(args)
            .WithParsedAsync<PipelineOptions>(Batched3DPipeline).ConfigureAwait(false);

    }

    private static async Task Batched3DPipeline(PipelineOptions ops)
    {
        Pipeline pipeline = new Pipeline(ops);
        await pipeline.Process();
    }
}