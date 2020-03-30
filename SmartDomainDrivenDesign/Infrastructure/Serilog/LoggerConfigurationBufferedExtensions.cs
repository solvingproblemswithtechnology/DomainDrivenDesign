using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using System;

namespace SmartDomainDrivenDesign.Infrastructure.Serilog
{
    public static class LoggerConfigurationBufferedExtensions
    {
        public static LoggerConfiguration Buffer(this LoggerSinkConfiguration loggerSinkConfiguration,
            Action<LoggerSinkConfiguration> configure, LogEventLevel minimumLevel = LogEventLevel.Information,
            LogEventLevel fullDumpLevel = LogEventLevel.Error, LoggingLevelSwitch controlLevelSwitch = null)
        {
            return loggerSinkConfiguration.Async(builder => LoggerSinkConfiguration.Wrap(
                builder, wrappedSink => new BufferedSink(wrappedSink, minimumLevel, fullDumpLevel, controlLevelSwitch),
                    configure, LevelAlias.Minimum, null));
        }
    }
}
