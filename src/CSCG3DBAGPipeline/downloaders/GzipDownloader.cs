using System.Net;

namespace CSCG3DBAGPipeline;

public class GzipDownloader : AbstractDownloader
{
    // Zoals geschreven in de remarks dient een HttpClient hergebruikt te worden:
    // (https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-6.0#remarks)
    private static readonly HttpClient _client;

    static GzipDownloader()
    {
        HttpClientHandler handler = new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip
        };
        _client = new HttpClient(handler);
    }

    /// <summary>
    /// GzipDownloader class.
    /// </summary>
    /// <param name="outDir">The output directory.</param>
    public GzipDownloader(string outDir) : base(outDir)
    {
    }

    /// <summary>
    /// Downloads a file from a uri and saves it to our output directory (_outDir).
    /// </summary>
    /// <param name="fileUrl">The file uri.</param>
    /// <param name="saveAs">The name with which the download file will be saved.</param>
    public override async Task DownloadFile(string fileUrl, string saveAs)
    {
        using (var stream = await _client.GetStreamAsync(fileUrl))
        {
            // TODO: Make it check if version of file already exits so that we don't redownload everything (check if file exists)
            using (var fileStream = new FileStream(Path.Combine(this.OutDir, saveAs), FileMode.Create))
            {
                await stream.CopyToAsync(fileStream);
            }
        }
    }
}