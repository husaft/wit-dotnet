using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Wit.Tools
{
    public static class WitLog
    {
        private static ILoggerFactory GetFactory(LogLevel minLevel)
        {
            var provider = new ServiceCollection()
                .AddLogging(builder => builder
                    .SetMinimumLevel(minLevel)
                    .AddConsole()
                )
                .BuildServiceProvider();
            return provider.GetService<ILoggerFactory>();
        }

        public static ILogger<T> Create<T>(LogLevel minLevel
            = LogLevel.Information)
        {
            var factory = GetFactory(minLevel);
            return factory.CreateLogger<T>();
        }

        public static ILogger<WitClient> Create(LogLevel minLevel
            = LogLevel.Information) => Create<WitClient>(minLevel);
    }
}