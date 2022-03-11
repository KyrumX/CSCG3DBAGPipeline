using CliWrap;
using CliWrap.Buffered;

namespace CSCG3DBAGPipeline.processing;

public class GLBProcessor : Processor
{
    private readonly string _applyDracoFile;
    
    /// <summary>
    /// Constructor for our GLBProcessor. This class processes functions related to GLB.
    /// </summary>
    /// <param name="applyDracoFile">Filename of the .bat file which applies Draco compression to binary GLB.</param>
    /// <param name="workingDir">The path to our working directory, from here we will traverse.</param>
    public GLBProcessor(string applyDracoFile, string workingDir) : base(workingDir)
    {
        this._applyDracoFile = applyDracoFile;
    }

    /// <summary>
    /// Takes a binary GLB file and applies Draco compression. Uses the specified applyDracoFile, which is a .bat.
    /// </summary>
    /// <param name="inFilePath">Filename (with extension) used as input, relative to the working directory.</param>
    /// <param name="outFilePath">Filename (with extension) of output file, relative to the working directory.</param>
    /// <returns>Task type CommandResult</returns>
    public async Task<CommandResult> ApplyDracoCompression(string inFilePath, string outFilePath)
    {
        Command cmd = this.CommandBuilder(this._applyDracoFile, new[] { inFilePath, outFilePath });
        CommandResult res = await cmd.ExecuteBufferedAsync();

        return res;
    }

    /// <summary>
    /// Takes a binary GLB file and converts it to a Batched 3D Model.
    /// </summary>
    /// <param name="inFilePath">Filename (with extension) used as input, relative to the working directory.</param>
    /// <param name="outFilePath">Filename (with extension) of output file, relative to the working directory.</param>
    public void ToBatched3DModel(string inFilePath, string outFilePath)
    {
        // Bouw de paden
        string glbFilePath = Path.Combine(this._workingDirectory, inFilePath);
        string outB3dmFilePath = Path.Combine(this._workingDirectory, outFilePath);
        
        // Lees GLB
        byte[] glb = File.ReadAllBytes(glbFilePath);
        
        // Maak Batched 3D Model
        var b3dm = new B3dm.Tile.B3dm(glb);
        var b3dmBytes = b3dm.ToBytes();
        
        // Schrijf Batched 3D Model naar het opgegeven pad met de opgegeven naam
        File.WriteAllBytes(outB3dmFilePath, b3dmBytes);
    }
}