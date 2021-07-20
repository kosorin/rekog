using System.Text;
using Rekog.Core.Corpora;
using Shouldly;
using Xunit;

namespace Rekog.Core.UnitTests.Corpora
{
    public class NgramCollectorTests
    {
        [Fact]
        public void Occurrences()
        {
            var parser = new NgramParser(2);
            var collector = new NgramCollector(parser);
            var corpus = "abcd ef efx";
            var expectedNgramOccurrences = new[]
            {
                new Occurrence<string>("ab", 1),
                new Occurrence<string>("bc", 1),
                new Occurrence<string>("cd", 1),
                new Occurrence<string>("ef", 2),
                new Occurrence<string>("fx", 1),
            };

            foreach (var character in corpus.EnumerateRunes())
            {
                if (Rune.IsWhiteSpace(character))
                {
                    collector.Skip();
                }
                else
                {
                    collector.Next(character);
                }
            }
            var ngramOccurrences = collector.Occurrences;

            ngramOccurrences.ShouldBe(expectedNgramOccurrences, ignoreOrder: true);
        }
        
        [Fact]
        public void Occurrences_UnicodeCharacters()
        {
            var parser = new NgramParser(2);
            var collector = new NgramCollector(parser);
            var corpus = "🌄⛔🚕x";
            var expectedNgramOccurrences = new[]
            {
                new Occurrence<string>("🌄⛔", 1),
                new Occurrence<string>("⛔🚕", 1),
                new Occurrence<string>("🚕x", 1),
            };

            foreach (var character in corpus.EnumerateRunes())
            {
                if (Rune.IsWhiteSpace(character))
                {
                    collector.Skip();
                }
                else
                {
                    collector.Next(character);
                }
            }
            var ngramOccurrences = collector.Occurrences;

            ngramOccurrences.ShouldBe(expectedNgramOccurrences, ignoreOrder: true);
        }
    }
}
