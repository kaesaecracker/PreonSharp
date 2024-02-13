namespace PreonSharp.Loaders;

public class SepFileSpec
{
    public required string FilePath { get; init; }

    public required char Separator { get; init; }

    public required int NameColumnIndex { get; init; }

    public required int IdColumnIndex { get; init; }

    public bool HasHeader { get; init; } = true;

    public bool Unquote { get; init; } = false;
}
