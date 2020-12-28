using Rekog.Core;
using Rekog.Core.Corpora;
using Rekog.Core.Layouts;
using Rekog.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;

namespace Rekog
{
    public class LayoutController
    {
        private readonly Options _options;
        private readonly Config _config;
        private readonly IConsole _console;
        private readonly ILogger _logger;

        public LayoutController(Options options, Config config, IConsole console, ILogger logger)
        {
            _options = options;
            _config = config;
            _console = console;
            _logger = logger.ForContext<LayoutController>();
        }

        public void Analyze(CorpusData corpusData)
        {
            var layout = GetLayout();
            if (layout == null)
            {
                return;
            }

            var corpusAnalysis = new CorpusAnalysis(corpusData);

            var layoutAnalyzer = new LayoutAnalyzer();
            layoutAnalyzer.Analyze(corpusAnalysis, layout);
        }

        private Layout? GetLayout()
        {
            var keymapConfig = _config.Keymaps[_options.Keymap];
            var layoutConfig = _config.Layouts[_options.Layout];

            var keys = new Dictionary<char, Key>();
            for (var row = 0; row < layoutConfig.Fingers.Count; row++)
            {
                for (var column = 0; column < layoutConfig.Fingers[row].Count; column++)
                {
                    var fingerValue = layoutConfig.Fingers[row][column];
                    if (!fingerValue.HasValue || !Enum.GetValues<Finger>().Contains((Finger)fingerValue.Value))
                    {
                        _logger.Error("Bad layout finger format: {FingerValue}", fingerValue);
                        return null;
                    }

                    var finger = (Finger)fingerValue.Value;
                    foreach (var layerConfig in keymapConfig.Layers)
                    {
                        var character = layerConfig.Keys[row][column];
                        if (!character.HasValue)
                        {
                            continue;
                        }

                        var key = new Key(char.ToUpperInvariant(character.Value), finger, row, column);
                        if (!keys.TryAdd(key.Character, key))
                        {
                            _logger.Warning("Multiple layout character {Character}", key.Character);
                            continue;
                        }
                    }
                }
            }

            var layout = new Layout(keys);

            _logger.Information("Parsed layout {Layout} with keymap {Keymap}", _options.Layout, _options.Keymap);

            return layout;
        }
    }
}
