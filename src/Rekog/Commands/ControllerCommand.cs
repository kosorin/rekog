﻿using Rekog.Controllers;
using Rekog.Persistence;
using Rekog.Persistence.Serialization;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.IO;
using System.IO.Abstractions;
using System.Threading;
using System.Threading.Tasks;

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

            Handler = CommandHandler.Create<TOptions, IConsole, CancellationToken>(HandleAsync);

            AddArgument(new Argument<string>("config"));
            AddOption(new Option<string>(new[] { "--output", "-o" }) { IsRequired = true });
        }

        protected IFileSystem FileSystem { get; }

        protected abstract SerializerBase<TConfig> GetConfigSerializer();

        private TConfig? BuildConfig(TOptions options)
        {
            try
            {
                using var stream = FileSystem.FileStream.Create(options.Config, FileMode.Open);
                using var reader = new StreamReader(stream);
                var config = GetConfigSerializer().Deserialize(reader);
                config.Options = options;
                config.Fix();
                return config;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private Task<int> HandleAsync(TOptions options, IConsole console, CancellationToken cancellationToken)
        {
            var config = BuildConfig(options);
            if (config == null)
            {
                console.Error.WriteLine("Could not load or parse config");
                return Task.FromResult(1);
            }

            if (Activator.CreateInstance(typeof(TController), config, console, FileSystem) is not TController controller)
            {
                return Task.FromResult(1);
            }

            return controller.HandleAsync(cancellationToken);
        }
    }
}
