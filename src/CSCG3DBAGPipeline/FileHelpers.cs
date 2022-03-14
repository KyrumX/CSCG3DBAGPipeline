namespace CSCG3DBAGPipeline;

static class FileHelpers
{
    public static bool DoesFileExist(string workingDir, string filePath)
    {
        string path = @Path.Combine(workingDir, filePath);
        return File.Exists(path);
    }

    /// <summary>
    /// Deletes a file.
    /// </summary>
    /// <param name="path">Path to the file which should be deleted.</param>
    /// <param name="workingDir">Optional workingDirectory, in this case the path should be relative to this dir.</param>
    /// <returns>
    ///     Boolean, true if no exceptions were thrown, false if exceptions were thrown and file thus
    ///     wasn't deleted.
    /// </returns>
    public static bool DeleteFile(string path, string workingDir = "")
    {
        string fullPath = Path.Combine(workingDir, path);
        try
        {
            File.Delete(@fullPath);
            return true;
        }
        catch (Exception e)
        {
            // TODO: LOG
            Console.WriteLine(e);
            return false;
        }
    }

    /// <summary>
    /// Accepts an IEnumerable of strings of file paths which should be deleted.
    /// </summary>
    /// <param name="paths">Strings of files which should be deleted, IEnumerable of strings.</param>
    /// <param name="workingDir">Optional workingDirectory, in this case the paths should be relative to this dir.</param>
    public static void DeleteFiles(IEnumerable<string> paths, String workingDir = "")
    {
        foreach (string path in paths)
        {
            DeleteFile(path, workingDir);
        }
    }
}