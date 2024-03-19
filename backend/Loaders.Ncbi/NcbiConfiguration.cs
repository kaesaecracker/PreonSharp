namespace Loaders.Ncbi;

public class NcbiConfiguration
{
    public string? TaxonomyDataRoot { get; set; }

    public GeneInfo GeneInfo { get; } = new();
}

public class GeneInfo
{
    public string? File { get; set; }

    public bool LoadExtendedTags { get; set; }

    public uint? Sample { get; set; }
}