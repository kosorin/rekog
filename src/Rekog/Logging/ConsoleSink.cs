using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;
using System;
using System.CommandLine;
using System.CommandLine.IO;
using System.Globalization;
using System.IO;

namespace Rekog.Logging
{
    public class ConsoleSink : ILogEventSink
    {
        private readonly IConsole _console;
        private readonly ITextFormatter _textFormatter;
        private readonly IFormatProvider _formatProvider;

        public ConsoleSink(IConsole console, ITextFormatter textFormatter, IFormatProvider formatProvider)
        {
            _console = console;
            _textFormatter = textFormatter;
            _formatProvider = formatProvider;
        }

        public void Emit(LogEvent logEvent)
        {
            var writer = GetWriter(logEvent);
            var message = GetMessage(logEvent);
            writer.Write(message);
        }

        private IStandardStreamWriter GetWriter(LogEvent logEvent)
        {
            return logEvent.Level >= LogEventLevel.Error ? _console.Error : _console.Out;
        }

        private string GetMessage(LogEvent logEvent)
        {
            using var buffer = new StringWriter(_formatProvider);
            _textFormatter.Format(logEvent, buffer);
            return buffer.ToString();
        }
    }

    public static class ConsoleSinkExtensions
    {
        public static LoggerConfiguration Console(this LoggerSinkConfiguration sinkConfiguration, IConsole console, LogEventLevel restrictedToMinimumLevel, string outputTemplate)
        {
            var formatProvider = CultureInfo.InvariantCulture;
            var textFormatter = new MessageTemplateTextFormatter(outputTemplate, formatProvider);
            return sinkConfiguration.Sink(new ConsoleSink(console, textFormatter, formatProvider), restrictedToMinimumLevel);
        }
    }
}
