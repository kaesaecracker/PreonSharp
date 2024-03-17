namespace Taxonomy.Models;

public record TextMatch(string Text, ISet<Guid> EntityIds);