namespace CSCG3DBAGPipeline;

static class FileHelpers
{
    public static bool DoesFileExist(string workingDir, string filePath)
    {
        string path = @Path.Combine(workingDir, filePath);
        return File.Exists(path);
    }
}