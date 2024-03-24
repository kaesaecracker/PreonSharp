using System.Diagnostics;
using Microsoft.Extensions.Options;

namespace Taxonomy;

public class ProgressWatcherConfiguration
{
    public int Delay { get; set; } = 1000;
}

public sealed class ProgressWatcherFactory(IOptions<ProgressWatcherConfiguration> configuration)
{
    private readonly int _delay = configuration.Value.Delay;

    public Task Indeterminate(Task taskToWatch, ILogger logger, Func<int> func) => Task.Factory.StartNew(async () =>
    {
        var overallTimeWatch = Stopwatch.StartNew();
        var lastValue = 0;

        while (!taskToWatch.IsCompleted)
        {
            await Task.Delay(_delay);

            var newValue = func();
            if (lastValue == newValue)
                continue;

            lastValue = newValue;
            logger.LogDebug("task running at value {} in {}", newValue, overallTimeWatch.Elapsed);
        }

        logger.LogInformation("task done with end value {} in {}", func(), overallTimeWatch.Elapsed);
    });
}