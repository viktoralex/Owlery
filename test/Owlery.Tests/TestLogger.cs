using System;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Owlery.Tests
{
    public class TestLogger
    {
        public static ILogger<T> Create<T>()
        {
            return new IgnoreOutputTestLogger<T>();
            //return new UnitTestLogger<T>();
        }

        public static ILogger<T> CreateXUnit<T>(ITestOutputHelper output)
        {
            return new XUnitTestLogger<T>(output);
        }

        class UnitTestLogger<T> : ILogger<T>, IDisposable
        {
            private readonly Action<string> output = Console.WriteLine;

            public void Dispose()
            {
            }

            public IDisposable BeginScope<TState>(TState state) => this;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) =>
                output(formatter(state, exception));
        }

        class XUnitTestLogger<T> : ILogger<T>, IDisposable
        {
            private readonly Action<string> output;

            public XUnitTestLogger(ITestOutputHelper output)
            {
                this.output = output.WriteLine;
            }

            public void Dispose()
            {
            }

            public IDisposable BeginScope<TState>(TState state) => this;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) =>
                output(formatter(state, exception));
        }

        class IgnoreOutputTestLogger<T> : ILogger<T>, IDisposable
        {
            public void Dispose()
            {
            }

            public IDisposable BeginScope<TState>(TState state) => this;

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) {}
        }
    }
}