namespace Taxonomy;

public interface IStartAwaitable
{
    Task Started { get; }
}