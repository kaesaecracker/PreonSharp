namespace PreonSharp;

[Flags]
public enum MatchType : byte
{
    Partial = 0b0001,
    Exact = 0b0010,
    Substring = 0b0100,
    All = Partial | Exact | Substring,
}
