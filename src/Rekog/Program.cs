using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Threading;
using Autofac;
using Rekog.Controllers;
using Rekog.Data;
using Rekog.Data.Serialization;
using Rekog.Logging;
using Serilog;
using Serilog.Events;

namespace Rekog
{
    internal class Program
    {
        private readonly ILogger _logger;
        private readonly SystemConsole _console;
        private readonly FileSystem _fileSystem;

        private Program()
        {
            _console = new SystemConsole();
            _logger = CreateLogger(_console);
            _fileSystem = new FileSystem();
        }

        private static void Main(string[] args)
        {
            new Program().Run(args);
        }

        private void Run(string[] args)
        {
            try
            {
                BuildParser().Invoke(args, _console);
            }
            catch (Exception e)
            {
                _logger.Fatal(e, "Unhandled exception");
            }
        }

        private ILogger CreateLogger(IConsole console)
        {
            var outputTemplate = "[{Timestamp:HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}";
            return new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Debug(LogEventLevel.Debug, outputTemplate)
                .WriteTo.Console(console, LogEventLevel.Debug, outputTemplate)
                .CreateLogger();
        }

        private Parser BuildParser()
        {
            return new CommandLineBuilder(GetCommand())
                .UseVersionOption()
                .UseHelp()
                .UseParseErrorReporting()
                .UseSuggestDirective()
                .RegisterWithDotnetSuggest()
                .CancelOnProcessTermination()
                .Build();
        }

        private Command GetCommand()
        {
            var command = new RootCommand();

            command.AddOption(new Option<string>(new[] { "--alphabet", "-a", }) { IsRequired = true, });
            command.AddOption(new Option<string>(new[] { "--corpus", "-c", }) { IsRequired = true, });
            command.AddOption(new Option<string>(new[] { "--keymap", "-k", }) { IsRequired = true, });
            command.AddOption(new Option<string>(new[] { "--layout", "-l", }) { IsRequired = true, });

            command.Handler = CommandHandler.Create<Options, CancellationToken>(Handle);

            return command;
        }

        private void Handle(Options options, CancellationToken cancellationToken)
        {
            PrepareOptions(options);
            var config = LoadConfig();

            var container = BuildContainer(options, config);

            container.Resolve<ProgramController>().Run(cancellationToken);
        }

        [SuppressMessage("ReSharper", "ConstantNullCoalescingCondition")]
        private void PrepareOptions(Options options)
        {
            options.Corpus ??= string.Empty;
            options.Alphabet ??= string.Empty;
            options.Layout ??= string.Empty;
            options.Keymap ??= string.Empty;

            _logger.Debug("Parsed command line options");
        }

        private Config LoadConfig()
        {
            var configFile = new DataFile(_fileSystem, "config.yml");
            using var reader = configFile.GetReader();
            var config = new ConfigSerializer().Deserialize(reader);

            _logger.Debug("Loaded config {ConfigFile}", configFile.Path);

            return config;
        }

        private IContainer BuildContainer(Options options, Config config)
        {
            var builder = new ContainerBuilder();

            builder.RegisterInstance(options).As<Options>();
            builder.RegisterInstance(config).As<Config>();
            builder.RegisterInstance(_logger).As<ILogger>();
            builder.RegisterInstance(_console).As<IConsole>();
            builder.RegisterInstance(_fileSystem).As<IFileSystem>();
            builder.RegisterType<CorpusController>();
            builder.RegisterType<LayoutController>();
            builder.RegisterType<ProgramController>();

            return builder.Build();
        }
    }
}
