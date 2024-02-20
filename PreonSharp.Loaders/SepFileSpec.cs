namespace PreonSharp.Loaders;

public class SepFileSpec
{
    public string? FilePath { get; set; }

    public char? Separator { get; set; }

    public int? NameColumnIndex { get; set; }

    public int? IdColumnIndex { get; set; } 

    public bool HasHeader { get; set; } = true;

    public bool Unquote { get; set; }
}