namespace PreonSharpTests;

public static class TestHelpers
{
    public static IServiceProvider BuildTestEnvironment(Action<INormalizerBuilder> configure)
    {
        var serviceCollection = new ServiceCollection();
        
        serviceCollection.AddSingleton<ILoggerFactory, NullLoggerFactory>();
        serviceCollection.Add(ServiceDescriptor.Singleton(typeof(ILogger<>), typeof(NullLogger<>)));

        serviceCollection.AddNormalizer(configure);
        
        return serviceCollection.BuildServiceProvider();
    }
}