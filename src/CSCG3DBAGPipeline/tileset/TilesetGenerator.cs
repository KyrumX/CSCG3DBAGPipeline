using System.Text.Json;
using System.Text.RegularExpressions;
using CSCJConverter;
using CSCJConverter.tileset;
using Serilog;

namespace CSCG3DBAGPipeline.tileset;

public class TilesetGenerator
{
    private readonly TilesetGeneratorOptions _options;
    private readonly string[] _files;
    private AbstractTileset _tileset;
    public TilesetGenerator(TilesetGeneratorOptions options)
    {
        this._options = options; 
        Console.WriteLine(_options.CityJSONPath);
        this._files = this.FilesToBeAdded(this._options.CityJSONPath, this._options.CityJSONFileRegex);

        // Factory voor verschillende types, mocht dat ooit nodig worden:
        this._tileset = TilesetFactory.FTileset(options);
    }

    /// <summary>
    /// Returns an array of string with all file names in a directory matching our regex.
    /// </summary>
    /// <param name="pathToFolder">The path to the folder.</param>
    /// <param name="regex">The regex used for filtering file names.</param>
    /// <returns>An array containing all the filenames that should be added to the tileset.</returns>
    private string[] FilesToBeAdded(string @pathToFolder, string @regex)
    {
        try
        {
            Regex rx = new Regex(this._options.CityJSONFileRegex);
        
            // Vind alle files in de directory welke voldoen aan de regex, sla enkel het bestandnaam op (standaard slaat het het gehele path op)
            string[] files = Directory.EnumerateFiles(pathToFolder).Where(path => rx.Match(Path.GetFileName(path)).Success)
                .Select(Path.GetFileName).ToArray();

            return files;
        }
        catch (Exception e)
        {
            Log.Error(e, "Unable to load/find files: see error message for details.");
            return Array.Empty<string>();
        }
    }
    
    /// <summary>
    /// Add the CityJSON files as tiles to our tileset object.
    /// </summary>
    public void AddTiles()
    {
        foreach (string file in this._files)
        {
            try
            {
                Console.WriteLine($"Now processing {file}...");
                // Bouw het volledige path
                string filePath = Path.Combine(this._options.CityJSONPath, file);
                // Haal het tegel nummer uit de naam van het bestand (pakt standaard eerste group)
                string tileNum = Regex.Match(file, this._options.TileNumberRegex).Groups[0].Value;
                // Bouw de B3DM content uri voor deze tileset entry
                string b3dmPath = String.Format(this._options.B3dmPathFormatString, tileNum.ToString());
                Console.WriteLine(b3dmPath);
                // Lees het CityJSON bestand
                string jsonFile = File.ReadAllText(@filePath);
                // Pak de geografische omvang
                double[] geographicalExtent = JsonSerializer.Deserialize<CityJSONModel>(jsonFile).metadata.geographicalExtent;
                // Voeg de geografische omvang toe aan de tegel met de content URI
                this._tileset.AddTile(geographicalExtent, b3dmPath);
                
                Console.WriteLine($"Successfully added {file} to the tileset object.");
            }
            catch (Exception e)
            {
                Log.Error(e, $"Encountered an error while trying to add {file} to the tileset object!");
            }
        }
    }

    /// <summary>
    /// Generate our tileset object, serialize it and return true if no error occurred.
    /// </summary>
    /// <returns>Boolean, if true everything went alright, if false something went wrong.</returns>
    public bool SerializeTileset()
    {
        if (!this._files.Any())
        {
            Log.Error("No new tiles found. Not generating a new tileset.");
            return false;
        }
        if (this._tileset.CountTiles() > 0)
        {
            try
            {
                TilesetModel model = this._tileset.GenerateTileset();
                File.WriteAllText(Path.Combine(this._options.TilesetOutPath, this._options.TilesetName), JsonSerializer.Serialize<TilesetModel>(model));
                Console.WriteLine("Successfully created tileset!");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Log.Error(e, "Failed to create a Serialized Tileset file. See log for more information.");
                return false;
            }
        }
        Log.Error("Unable to generate tileset, no (new) tiles available.");
        return false;
    }
}