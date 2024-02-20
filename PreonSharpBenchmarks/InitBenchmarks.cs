using PreonSharpTests;

namespace PreonSharpBenchmarks;

public class InitBenchmarks
{
    [ParamsSource(nameof(TestEnvironments))]
    public IServiceProvider? TestEnvironment { get; set; }

    private static readonly int[] Counts = [0, 10, 100, 1000];

    public IEnumerable<IServiceProvider> TestEnvironments => Counts.Select(n =>
        TestHelpers.BuildTestEnvironment(builder =>
        {
            builder.AddSeries(Enumerable.Range(0, n).Select(i => "name " + i),
                Enumerable.Range(0, n).Select(i => "id " + i));
        })
    );

    [Benchmark]
    public INormalizer BuildNormalizer()
    {
        return TestEnvironment!.GetRequiredService<INormalizer>();
    }
}