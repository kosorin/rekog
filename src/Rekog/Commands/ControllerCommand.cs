using Rekog.Controllers;
using Rekog.Persistence;
using Rekog.Persistence.Serialization;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.IO.Abstractions;
using System.Threading;

namespace Rekog.Commands
{
    public abstract class ControllerCommand<TConfig, TOptions, TController> : Command
        where TConfig : CommandConfig<TOptions>
        where TOptions : CommandOptions
        where TController : CommandController<TConfig, TOptions>
    {
        protected ControllerCommand(IFileSystem fileSystem, string name, string? description = null) : base(name, description)
        {
            FileSystem = fileSystem;

            Handler = CommandHandler.Create<TOptions, IConsole, CancellationToken>(Handle);

            AddArgument(new Argument<string>("config"));
            AddOption(new Option<string>(new[] { "--output", "-o" }) { IsRequired = true });
        }

        protected IFileSystem FileSystem { get; }

        protected abstract SerializerBase<TConfig> GetConfigSerializer();

        private TConfig BuildConfig(TOptions options)
        {
            using var stream = FileSystem.FileStream.Create(options.Config, FileMode.Open);
            using var reader = new StreamReader(stream);
            var config = GetConfigSerializer().Deserialize(reader);
            config.Options = options;
            config.Fix();
            return config;
        }

        private int Handle(TOptions options, IConsole console, CancellationToken cancellationToken)
        {
            var config = BuildConfig(options);

            if (Activator.CreateInstance(typeof(TController), config, console, FileSystem) is not TController controller)
            {
                return 1;
            }

            return controller.Handle(cancellationToken);
        }
    }
}
