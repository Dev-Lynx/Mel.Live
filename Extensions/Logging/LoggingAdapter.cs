using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mel.Live.API.Extensions.Logging
{
    internal class LoggingAdapter<T> : ILogger<T>
    {
        #region Properties

        #region Internals
        ILogger Adaptee { get; }
        #endregion

        #endregion

        #region Constructors
        public LoggingAdapter(ILoggerFactory factory)
        {
            Adaptee = factory.CreateLogger<T>();
        }
        #endregion

        #region Methods
        public IDisposable BeginScope<TState>(TState state)
            => Adaptee.BeginScope(state);

        public bool IsEnabled(LogLevel logLevel)
            => Adaptee.IsEnabled(logLevel);

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            => Adaptee.Log(logLevel, eventId, state, exception, formatter);
        #endregion

    }
}
