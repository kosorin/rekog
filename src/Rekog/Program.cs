using Rekog.Data;
using Rekog.Data.Serialization;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.IO.Abstractions;
using System.Threading;

namespace Rekog
{
    internal static class Program
    {
        private static readonly FileSystem FileSystem = new();

        // TODO: Handle errors/exceptions and return correct code (e.g. return 1 on error)
        private static int Main(string[] args)
        {
            return BuildParser().Invoke(args);
        }

        private static Parser BuildParser()
        {
            return new CommandLineBuilder(GetCommand())
                .UseDefaults()
                .Build();
        }

        private static Command GetCommand()
        {
            var command = new RootCommand();

            command.AddOption(new Option<string>(new[] { "--data-root", "-d" }));
            command.AddOption(new Option<string>(new[] { "--alphabet", "-a" }) { IsRequired = true });
            command.AddOption(new Option<string>(new[] { "--corpus", "-c" }) { IsRequired = true });
            command.AddOption(new Option<string>(new[] { "--keymap", "-k" }) { IsRequired = true });
            command.AddOption(new Option<string>(new[] { "--layout", "-l" }) { IsRequired = true });

            command.Handler = CommandHandler.Create<Options, IConsole, CancellationToken>(Handle);

            return command;
        }

        private static void Handle(Options options, IConsole console, CancellationToken cancellationToken)
        {
            var dataFileManager = new DataFileManager(options.DataRoot, FileSystem);
            var config = BuildConfig(options, dataFileManager);
            var controller = new Controller(config, dataFileManager, console, FileSystem);
            controller.Handle(cancellationToken);
        }

        private static Config BuildConfig(Options options, DataFileManager dataFileManager)
        {
            using var reader = dataFileManager.GetConfigReader();
            var config = new ConfigSerializer().Deserialize(reader);
            config.Options = options;
            config.FixAll();
            return config;
        }
    }
}
