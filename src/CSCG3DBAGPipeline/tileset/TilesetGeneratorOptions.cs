using CommandLine;

namespace CSCG3DBAGPipeline.tileset;

[Verb("tileset", HelpText = "Generate a tileset based on CityJSON files.")]
public class TilesetGeneratorOptions
{
    private string _outputDirectory;
    [Option('o', "outdir", Required = false, Default = "", HelpText = "Directory where the tileset will be saved. Empty will result in the current directory being used.")]
    public string TilesetOutPath
    {
        get => _outputDirectory;
        init
        {
            var path = string.IsNullOrEmpty(value) ? System.AppDomain.CurrentDomain.BaseDirectory : value;
            Directory.CreateDirectory(path);
            _outputDirectory = path;
        }
    }

    [Option('n', "name", Required = false, Default= "tileset.json", HelpText = "The name of the tileset. Please include .json extension: 'mytilesetname.json'")]
    public string TilesetName { get; init; }
    
    [Option('c', "cityjsondir", Required = true, HelpText = "Directory from where CityJSON files will be read.")]
    public string CityJSONPath { get; init; }

    private string _cityjsonFileRegex;
    [Option("cityjsonregex", Required = false, Default = @"^moved_\d+.json$",
        HelpText =
            "Regex used to pick which CityJSON files will be processed. By default will look for files named: 'moved_{number}.json'")]
    public string CityJSONFileRegex
    { get => _cityjsonFileRegex; init => _cityjsonFileRegex = @value;
    }

    private string _tileNumberRegex;
    [Option("tilenumregex", Required = false, Default = @"\d+", HelpText = "Regex used to extract the tile number from the CityJSON file name, by default picks sequence of numbers. Will always pick the first sequences that matches! So if there are two seperate sequences in the file name that match, the first one will be used.")]
    public string TileNumberRegex { get => _tileNumberRegex; init => _tileNumberRegex = @value; }
    
    [Option("outb3dmformatstring", Required = false, Default = "{0}.b3dm", HelpText = "The content uri that will be used. This is relative to your tileset! By default will make the uri '{tilenumber}.b3dm'. Please provide string with '{0}' where the tile number should be! Example: 'b3dm_tile{0}.b3dm'")]
    public string B3dmPathFormatString { get; init; }
    
    [Option("tileseterror", Required = false, Default = 260, HelpText = "Geometric error of the tileset.")]
    public double TilesetGeometricError { get; init; }
    
    [Option("rooterror", Required = false, Default = 4.5398975185470771, HelpText = "Root tile geometric error..")]
    public double RootGeometricError { get; init; }
    
    [Option("tileerror", Required = false, Default = 2.3232, HelpText = "Geometric error per tile.")]
    public double TileGeometricError { get; init; }
}