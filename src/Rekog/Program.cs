using Autofac;
using Rekog.Controllers;
using Rekog.Data;
using Rekog.Data.Serialization;
using Rekog.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using System.IO.Abstractions;
using System.Threading;

namespace Rekog
{
    internal class Program
    {
        private readonly SystemConsole _console;
        private readonly ILogger _logger;

        private static void Main(string[] args)
        {
            new Program().Run(args);
        }

        private Program()
        {
            _console = new SystemConsole();
            _logger = CreateLogger(_console);
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
                .WriteTo.Console(console, LogEventLevel.Information, outputTemplate)
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

            command.AddOption(new Option<string>(new[] { "--alphabet", "-a" }) { IsRequired = true });
            command.AddOption(new Option<string>(new[] { "--corpus", "-c" }) { IsRequired = true });
            command.AddOption(new Option<string>(new[] { "--keymap", "-k" }) { IsRequired = true });
            command.AddOption(new Option<string>(new[] { "--layout", "-l" }) { IsRequired = true });

            command.Handler = CommandHandler.Create<Options, CancellationToken>(Handle);

            return command;
        }

        private void Handle(Options options, CancellationToken cancellationToken)
        {
            FixOptions(options);

            var container = BuildContainer(options);

            container.Resolve<ProgramController>().Run(cancellationToken);
        }

        private void FixOptions(Options options)
        {
            options.Corpus ??= string.Empty;
            options.Alphabet ??= string.Empty;
            options.Layout ??= string.Empty;
            options.Keymap ??= string.Empty;
        }

        private IContainer BuildContainer(Options options)
        {
            var fileSystem = new FileSystem();
            var config = LoadConfig(fileSystem);

            var builder = new ContainerBuilder();
            builder.RegisterInstance(_console).As<IConsole>();
            builder.RegisterInstance(_logger).As<ILogger>();
            builder.RegisterInstance(fileSystem).As<IFileSystem>();
            builder.RegisterInstance(config).As<Config>();
            builder.RegisterInstance(options).As<Options>();
            builder.RegisterType<CorpusController>();
            builder.RegisterType<LayoutController>();
            builder.RegisterType<ProgramController>();

            var container = builder.Build();

            return container;
        }

        private Config LoadConfig(FileSystem fileSystem)
        {
            using var reader = new DataFile(fileSystem, "config.yml").GetReader();
            return new ConfigSerializer().Deserialize(reader);
        }
    }
}
