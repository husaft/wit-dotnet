using System;
using Microsoft.Extensions.Logging;

namespace Wit.Tests.Util
{
    public sealed class TestLogger : ILogger, IDisposable
    {
        public IDisposable BeginScope<TState>(TState state) => this;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
            Exception exception, Func<TState, Exception, string> formatter)
        {
            throw new InvalidOperationException();
        }

        public void Dispose()
        {
        }
    }
}