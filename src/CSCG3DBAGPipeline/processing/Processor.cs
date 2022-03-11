using CliWrap;

namespace CSCG3DBAGPipeline.processing;

public abstract class Processor
{
    internal readonly string _workingDirectory;
    
    private protected Processor(string workingDir)
    {
        this._workingDirectory = workingDir;
    }

    /// <summary>
    /// Build a command, set Validation to None, add arguments (arguments are always strings!), and define the
    /// .bat file we will run.
    /// </summary>
    /// <param name="file">The .bat file that will be used for this command.</param>
    /// <param name="arguments">The arguments. Arguments are always strings. Array of strings.</param>
    /// <returns>A (CLI) Command which can be executed or altered.</returns>
    private protected Command CommandBuilder(string file, IEnumerable<string> arguments)
    {
        return Cli.Wrap(file)
            .WithValidation(CommandResultValidation.None)
            .WithArguments(arguments)
            .WithWorkingDirectory(this._workingDirectory);
    }
}