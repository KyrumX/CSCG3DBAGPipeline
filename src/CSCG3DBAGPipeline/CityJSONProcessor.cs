using CliWrap;
using CliWrap.Buffered;

namespace CSCG3DBAGPipeline;

public class CityJSONProcessor
{
    private readonly string upgradeFilterFile;
    private readonly string toGlbFile;
    private readonly string workingDirectory;

    /// <summary>
    /// Constructor for our CityJSON processor. This class processes CLI commands related to CityJSON.
    /// </summary>
    /// <param name="upgradeFilterFile">Filename of the .bat file which uses cjio to filter/upgrade CityJSON.</param>
    /// <param name="toGlbFile">Filename of the .bat file which converts CityJSON to binary GLB.</param>
    /// <param name="workingDir">The path to our working directory, from here we will traverse.</param>
    public CityJSONProcessor(string upgradeFilterFile, string toGlbFile, string workingDir)
    {
        this.upgradeFilterFile = upgradeFilterFile;
        this.toGlbFile = toGlbFile;
        this.workingDirectory = workingDir;
    }

    /// <summary>
    /// Build a command, set Validation to None, add arguments (arguments are always strings!), and define the
    /// .bat file we will run.
    /// </summary>
    /// <param name="file">The .bat file that will be used for this command.</param>
    /// <param name="arguments">The arguments. Arguments are always strings. Array of strings.</param>
    /// <returns></returns>
    private Command CommandBuilder(string file, string[] arguments)
    {
        return Cli.Wrap(file)
            .WithValidation(CommandResultValidation.None)
            .WithArguments(arguments)
            .WithWorkingDirectory(this.workingDirectory);
    }

    /// <summary>
    /// Upgrades a 3DBAG CityJSON file to version 1.1, removes all detail levels except 2.2 and removes unnecessary
    /// attributes from our CityObjects. We use the upgradeFilter (bat) file and provide it 2 arguments:
    /// The first argument is the input file, the second argument is name the new file will have. 
    /// </summary>
    /// <param name="inFilePath">Filename (with extension) used as input, relative to the working directory.</param>
    /// <param name="outFilePath">Filename (with extension) of output file, relative to the working directory.</param>
    /// <returns>Task type CommandResult</returns>
    /// TODO: PATH EN FILE SPLITSEN?
    public async Task<CommandResult> FirstStep(string inFilePath, string outFilePath)
    {
        Command cmd = this.CommandBuilder(this.upgradeFilterFile, new[] {inFilePath, outFilePath});
        CommandResult res = await cmd.ExecuteBufferedAsync();

        return res;
    }

    /// <summary>
    /// Converts a CityJSON file to binary glTF.
    /// </summary>
    /// <param name="inFilePath">Filename (with extension) used as input, relative to the working directory.</param>
    /// <param name="outFilePath">Filename (with extension) of output file, relative to the working directory.</param>
    /// <returns></returns>
    public async Task<CommandResult> SecondStep(string inFilePath, string outFilePath)
    {
        Command cmd = this.CommandBuilder(this.toGlbFile, new[] {inFilePath, outFilePath});
        CommandResult res = await cmd.ExecuteBufferedAsync();

        return res;
    }
}