namespace CSCG3DBAGPipeline;

static class Utils
{
    public static string CombineUri(string uri1, string uri2)
    {
        // Haal '/' op het einde weg
        uri1 = uri1.TrimEnd('/');
        // Voeg een '/' toe aan het begin van het tweede deel van de uri
        uri2 = uri2.TrimStart('/');
        return $"{uri1}/{uri2}";
    }
}