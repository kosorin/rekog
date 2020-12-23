using Rekog.Core;
using Rekog.Core.Corpora;
using Rekog.Core.Layouts;
using Rekog.Core.Layouts.Analyzers;
using Rekog.Data;
using Rekog.Data.Serialization;
using Rekog.Extensions;
using Rekog.IO;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Rekog
{
    public class LayoutController
    {
        private readonly Options _options;
        private readonly Config _config;
        private readonly IFileSystem _fileSystem;
        private readonly IConsole _console;

        public LayoutController(Options options, Config config, IFileSystem fileSystem, IConsole console)
        {
            _options = options;
            _config = config;
            _fileSystem = fileSystem;
            _console = console;
        }

        public void Analyze(CorpusData corpusData)
        {
            var corpusAnalysis = new CorpusAnalysis(corpusData);
            //var layout = GetLayout(alphabet);


            //var layoutAnalyzers = new LayoutAnalyzer[]
            //{
            //    new FingerFrequencyLayoutAnalyzer(),
            //    new HandFrequencyLayoutAnalyzer(),
            //    new RowFrequencyLayoutAnalyzer(),
            //};

            //foreach (var layoutAnalyzer in layoutAnalyzers)
            //{
            //    layoutAnalyzer.Analyze(corpusAnalysis, layout);
            //    layoutAnalyzer.Print(_console);
            //}

            //new NgramAnalyzer().Analyze(corpusAnalysis, layout);
        }

        public Layout GetLayout(Alphabet alphabet)
        {
            var keymapConfig = _config.Keymaps[_options.Keymap];
            foreach (var layerConfig in keymapConfig.Layers)
            {
                _console.Out.WriteLine(string.Join('\n', layerConfig.Keys.Select(x => string.Join(' ', x.Select(i => i ?? ' ')))));
                _console.Out.WriteLine();
            }

            var layoutConfig = _config.Layouts[_options.Layout];
            _console.Out.WriteLine(string.Join('\n', layoutConfig.Fingers.Select(x => string.Join(' ', x.Select(i => i)))));
            _console.Out.WriteLine();

            var keys = new Dictionary<char, Key>();
            for (var row = 0; row < layoutConfig.Fingers.Count; row++)
            {
                for (var column = 0; column < layoutConfig.Fingers[row].Count; column++)
                {
                    var finger = (Finger)layoutConfig.Fingers[row][column];
                    var isHoming = layoutConfig.Homing[row][column];
                    foreach (var layerConfig in keymapConfig.Layers)
                    {
                        var character = layerConfig.Keys[row][column];
                        if (!character.HasValue || !alphabet.Contains(character.Value))
                        {
                            continue;
                        }

                        var key = new Key(character.Value, finger, row);
                        if (!keys.TryAdd(character.Value, key))
                        {
                            _console.Error.WriteLine($"Warning: Multiple character {character.Value}");
                            continue;
                        }
                    }
                }
            }

            var missingCharacters = from a in alphabet.Select(x => char.ToUpperInvariant(x))
                                    join k in keys.Select(x => (char?)char.ToUpperInvariant(x.Key)) on a equals k into g
                                    from k in g.DefaultIfEmpty()
                                    where !k.HasValue
                                    select a;
            foreach (var missingCharacter in missingCharacters.Distinct())
            {
                _console.Error.WriteLine($"Warning: Missing character {missingCharacter}");
            }

            return new Layout(keys);
        }
    }
}
