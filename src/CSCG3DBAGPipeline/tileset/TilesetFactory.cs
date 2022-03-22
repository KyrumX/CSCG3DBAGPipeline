using System.Text.Json;
using CSCJConverter.tileset;
using Serilog;

namespace CSCG3DBAGPipeline.tileset;

/// <summary>
/// Factory class for generating Tileset objects.
/// </summary>
public static class TilesetFactory
{

    public static AbstractTileset FTileset(TilesetGeneratorOptions options)
    {
        // Bepaal of we een tegelset inladen of een gehele nieuwe gaan maken:
        if (options.InputTileset.Any())
        {
            return TilesetFactoryFromInputTileset(options);
        }
        else
        {
            return TilesetFactoryClean(options);
        }
    }
    
    private static AbstractTileset TilesetFactoryClean(TilesetGeneratorOptions options)
    {
        try
        {
            switch (options.Type.ToLower())
            {
                default: return new GridTileset(
                        version:options.Version,
                        tilesetVersion:options.TilesetVersion,
                        tilesetGeometricError:(decimal)options.TilesetGeometricError,
                        rootGeometricError:(decimal)options.RootGeometricError,
                        tileGeometricError:(decimal)options.TileGeometricError
                    );
                    break;
            }
        }
        catch (Exception e)
        {
            Log.Error(e, "An error occurred while trying to create a tileset object using the provided settings.");
            throw;
        }
    }
    
    private static AbstractTileset TilesetFactoryFromInputTileset(TilesetGeneratorOptions options)
    {
        try
        {
            // Lees eerst een bestaand tegelset bestand in
            string tilesetString = File.ReadAllText(@options.InputTileset);
            TilesetModel tilesetModel = JsonSerializer.Deserialize<TilesetModel>(tilesetString);
            
            switch (options.Type.ToLower())
            {
                default:
                    return new GridTileset(
                        tilesetModel,
                        version: options.Version,
                        tilesetVersion:options.TilesetVersion
                    );
                    break;
            }
        }
        catch (Exception e)
        {
            Log.Error(e, "An error occurred while trying to load an exisiting tileset.");
            throw;
        }
    }
}