namespace PreonBenchmarks;

public static class RunBenchmarks
{
    public static void Main(string[] args)
    {
        if (!(args.Contains("-m") || args.Contains("--memory")))
            args = args.Append("-m").ToArray();
        BenchmarkSwitcher.FromAssembly(typeof(RunBenchmarks).Assembly).Run(args);
    }
}