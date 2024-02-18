namespace PreonSharp;

[Flags]
public enum MatchType : byte
{
    None = 0,
    Partial = 1 << 0,
    Exact = 1 << 1,
    Substring = 1 << 2,
    All = Partial | Exact | Substring,
}