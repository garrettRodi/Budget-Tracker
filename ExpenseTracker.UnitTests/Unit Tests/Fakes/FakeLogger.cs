using System;
using Microsoft.Extensions.Logging;

namespace BudgetTracker.Tests.UnitTests.Fakes
{
    /// <summary>
    /// A minimal fake logger that implements ILogger<T>.
    /// It can be extended to record log messages if needed for tests.
    /// </summary>
    public class FakeLogger<T> : ILogger<T>
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            return NullScope.Instance;
        }

        public bool IsEnabled(LogLevel logLevel) => false; // or return true if you want to capture logs

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            // Do nothing. Optionally, you can store messages in a collection for later assertions.
        }
    }

    internal class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new NullScope();
        public void Dispose() { }
    }
}
