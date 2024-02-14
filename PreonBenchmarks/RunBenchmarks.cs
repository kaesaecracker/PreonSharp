namespace PreonBenchmarks;

public static class RunBenchmarks
{
    public static void Main(string[] args)
    {
        BenchmarkSwitcher.FromAssembly(typeof(RunBenchmarks).Assembly).Run(args);
    }
}