namespace Taxonomy.Models;

public sealed class IdNamespace
{
    internal IdNamespace(string name)
    {
        Name = name;
    }

    public string Name { get; }
}