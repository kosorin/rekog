using Rekog.Core;
using Rekog.Core.Ngrams;
using Shouldly;
using System.Linq;
using Xunit;

namespace Rekog.UnitTests.Core.Ngrams
{
    public class UnigramScannerTests
    {
        [Fact]
        public void Next_CaseInsensitive()
        {
            var caseSensitive = false;
            var alphabet = new Alphabet("ABCDEF");
            var scanner = new UnigramScanner(caseSensitive, alphabet);

            var ngramValues = GetNgramValues("aB abc aD Ecc DEf", scanner);

            ngramValues.ShouldBe(new[]
            {
                "A", "B",
                "A", "B", "C",
                "A", "D",
                "E", "C", "C",
                "D", "E", "F",
            });
        }

        [Fact]
        public void Next_CaseSensitive()
        {
            var caseSensitive = true;
            var alphabet = new Alphabet("ABCDEF");
            var scanner = new UnigramScanner(caseSensitive, alphabet);

            var ngramValues = GetNgramValues("aB abc aD Ecc DEf", scanner);

            ngramValues.ShouldBe(new[]
            {
                "a", "B",
                "a", "b", "c",
                "a", "D",
                "E", "c", "c",
                "D", "E", "f",
            });
        }

        [Fact]
        public void Next_MultipleInvalidCharacters()
        {
            var caseSensitive = false;
            var alphabet = new Alphabet("ABCDEF");
            var scanner = new UnigramScanner(caseSensitive, alphabet);

            var ngramValues1 = GetNgramValues("    AB   BCD   DE    ", scanner);

            ngramValues1.ShouldBe(new[]
            {
                "A", "B",
                "B", "C", "D",
                "D", "E",
            });
        }

        [Fact]
        public void Next_WithoutClear()
        {
            var caseSensitive = false;
            var alphabet = new Alphabet("ABCDEF");
            var scanner = new UnigramScanner(caseSensitive, alphabet);

            var ngramValues1 = GetNgramValues("ABC", scanner);
            var ngramValues2 = GetNgramValues("CDE", scanner);

            ngramValues1.ShouldBe(new[]
            {
                "A", "B", "C",
            });
            ngramValues2.ShouldBe(new[]
            {
                "C", "D", "E",
            });
        }

        [Fact]
        public void Next_WithClear()
        {
            var caseSensitive = false;
            var alphabet = new Alphabet("ABCDEF");
            var scanner = new UnigramScanner(caseSensitive, alphabet);

            var ngramValues1 = GetNgramValues("ABC", scanner);
            scanner.Clear();
            var ngramValues2 = GetNgramValues("CDE", scanner);

            ngramValues1.ShouldBe(new[]
            {
                "A", "B", "C",
            });
            ngramValues2.ShouldBe(new[]
            {
                "C", "D", "E",
            });
        }

        private string[] GetNgramValues(string input, UnigramScanner scanner)
        {
            return input
                .Select(character => scanner.Next(character, out var ngramValue) ? ngramValue : null)
                .Where(x => x != null)
                .Cast<string>()
                .ToArray();
        }
    }
}
