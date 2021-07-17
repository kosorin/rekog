using System;
using System.Collections.Generic;
using System.Linq;
using Rekog.Core;
using Rekog.Core.Corpora;
using Rekog.Core.Layouts;
using Rekog.Data;
using Serilog;

namespace Rekog.Controllers
{
    public class LayoutController
    {
        private readonly Options _options;
        private readonly Config _config;
        private readonly ILogger _logger;

        public LayoutController(Options options, Config config, ILogger logger)
        {
            _options = options;
            _config = config;
            _logger = logger.ForContext<LayoutController>();
        }

        public void Analyze(CorpusAnalysisData corpusAnalysisData)
        {
            var characters = corpusAnalysisData.UnigramOccurrences.Select(x => x.Value[0]).ToArray();
            var layout = BuildLayout(characters);
            if (layout == null)
            {
                return;
            }

            var layoutAnalyzer = new LayoutAnalyzer();
            layoutAnalyzer.Analyze(corpusAnalysisData, layout);
        }

        private Layout? BuildLayout(char[] characters)
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
                    var effort = layoutConfig.Efforts[row][column];
                    var isHoming = layoutConfig.Homing[row][column];

                    foreach (var (layerConfig, layer) in keymapConfig.Layers.Select((x, i) => (x, i)))
                    {
                        var character = layerConfig.Keys[row][column];
                        if (!character.HasValue)
                        {
                            continue;
                        }

                        var key = new Key(char.ToUpperInvariant(character.Value), finger, isHoming, layer, row, column, effort);
                        if (!keys.TryAdd(key.Character, key))
                        {
                            _logger.Warning("Character {Character} is specified multiple times", key.Character);
                        }
                    }
                }
            }

            var missingCharacters = characters.Except(keys.Select(x => x.Key)).ToList();
            if (missingCharacters.Any())
            {
                _logger.Warning("Following characters cannot be written: {MissingCharacters}", missingCharacters);
            }

            var layout = new Layout(keys);

            _logger.Information("Parsed layout {Layout} with keymap {Keymap}", _options.Layout, _options.Keymap);

            return layout;
        }
    }
}
