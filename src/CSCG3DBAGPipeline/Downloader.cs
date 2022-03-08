using System.Net;

namespace CSCG3DBAGPipeline;

public class Downloader
{
    // Zoals geschreven in de remarks dient een HttpClient hergebruikt te worden:
    // (https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-6.0#remarks)
    private static HttpClient _client;

    private readonly string _outDir ;
    
    static Downloader()
    {
        _client = new HttpClient();
    }

    public Downloader(string outDir)
    {
        this._outDir = outDir;
        Directory.CreateDirectory(this._outDir);
    }

    /// <summary>
    /// Downloads a file from a uri and saves it to our output directory (_outDir)
    /// </summary>
    /// <param name="fileUrl">The file uri</param>
    /// <param name="saveAs">The name with which the download file will be saved</param>
    public async Task DownloadFile(string fileUrl, string saveAs)
    {
        using (var stream = await _client.GetStreamAsync(fileUrl))
        {
            // TODO: Make it check if version of file already exits so that we don't redownload everything (check if file exists)
            using (var fileStream = new FileStream(this._outDir + "/" + saveAs, FileMode.Create))
            {
                await stream.CopyToAsync(fileStream);
            }
        }
    }
}