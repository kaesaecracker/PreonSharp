namespace PreonTests;

public class UnitTest1
{
    [Fact]
    public void EmptyNormalizerDoesNothing()
    {
        Assert.Null(
            TestHelpers.BuildTestEnvironment(builder => { })
                .GetRequiredService<INormalizer>()
                .QueryAsync("foo").Result
        );
    }
    
    [Fact]
    public void OneEntryNormalizerDoesOneThing()
    {
        var n = TestHelpers.BuildTestEnvironment(builder =>
            {
                builder.AddSeries("test", new[] { "abcdef" }, new[] { "id001" });
            })
            .GetRequiredService<INormalizer>();
        
        Assert.Null(n.QueryAsync("foo").Result);
        
        var queryResult = n.QueryAsync("abcdef").Result;
        Assert.NotNull(queryResult);
    }
}