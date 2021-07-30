using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            var characters = corpusAnalysisData.Unigrams.Select(x => x.Value.EnumerateRunes().Single()).ToArray();
            var layout = BuildLayout(characters);
            if (layout == null)
            {
                return;
            }

            new LayoutAnalyzer()
                .Analyze(corpusAnalysisData, layout)
                .Print();
        }

        private Layout? BuildLayout(Rune[] characters)
        {
            var keymapConfig = _config.Keymaps[_options.Keymap];
            var layoutConfig = _config.Layouts[_options.Layout];

            var keys = new Dictionary<Rune, Key>();
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
                        if (layerConfig.Keys[row][column] is not { } characterString)
                        {
                            continue;
                        }

                        foreach (var character in characterString.EnumerateRunes())
                        {
                            var key = new Key(Rune.ToUpperInvariant(character), finger, isHoming, layer, row, column, effort);
                            if (!keys.TryAdd(key.Character, key))
                            {
                                _logger.Warning("Character {Character} is specified multiple times", key.Character);
                            }
                        }
                    }
                }
            }

            var missingCharacters = characters.Except(keys.Keys).ToList();
            if (missingCharacters.Any())
            {
                _logger.Warning("Following characters are not specified in the layout: {MissingCharacters}", missingCharacters);
            }

            var layout = new Layout(keys);

            _logger.Information("Parsed layout {Layout} with keymap {Keymap}", _options.Layout, _options.Keymap);

            return layout;
        }
    }
}
