using Rekog.Core;
using Rekog.Core.Ngram;
using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace Rekog.UnitTests.Core.Ngrams
{
    public class NgramScannerTests
    {
        [Fact]
        public void Ctor_SizeLessThan1()
        {
            var alphabet = new Alphabet("ABCDEF");

            Should.Throw<ArgumentOutOfRangeException>(() => new NgramScanner(0, false, alphabet));
        }

        [Fact]
        public void Next_Size1()
        {
            var caseSensitive = false;
            var alphabet = new Alphabet("ABCDEF");
            var scanner = new NgramScanner(1, caseSensitive, alphabet);

            var ngramValues = GetNgramValues("AB CDEF EAF AZBC", scanner);

            ngramValues.ShouldBe(new[]
            {
                "A", "B",
                "C", "D", "E", "F",
                "E", "A", "F",
                "A", "B", "C",
            });
        }

        [Fact]
        public void Next_Size2()
        {
            var caseSensitive = false;
            var alphabet = new Alphabet("ABCDEF");
            var scanner = new NgramScanner(2, caseSensitive, alphabet);

            var ngramValues = GetNgramValues("AB CDEF EAF AZBC", scanner);

            ngramValues.ShouldBe(new[]
            {
                "AB",
                "CD", "DE", "EF",
                "EA", "AF",
                "BC",
            });
        }

        [Fact]
        public void Next_Size4()
        {
            var caseSensitive = false;
            var alphabet = new Alphabet("ABCDEF");
            var scanner = new NgramScanner(4, caseSensitive, alphabet);

            var ngramValues = GetNgramValues("AB CDEF EAFAB AZBC ABC", scanner);

            ngramValues.ShouldBe(new[]
            {
                "CDEF",
                "EAFA", "AFAB",
            });
        }

        [Fact]
        public void Next_CaseInsensitive()
        {
            var caseSensitive = false;
            var alphabet = new Alphabet("ABCDEF");
            var scanner = new NgramScanner(2, caseSensitive, alphabet);

            var ngramValues = GetNgramValues("aB abc aD Ecc DEf", scanner);

            ngramValues.ShouldBe(new[]
            {
                "AB",
                "AB", "BC",
                "AD",
                "EC", "CC",
                "DE", "EF",
            });
        }

        [Fact]
        public void Next_CaseSensitive()
        {
            var caseSensitive = true;
            var alphabet = new Alphabet("ABCDEF");
            var scanner = new NgramScanner(2, caseSensitive, alphabet);

            var ngramValues = GetNgramValues("aB abc aD Ecc DEf", scanner);

            ngramValues.ShouldBe(new[]
            {
                "aB",
                "ab", "bc",
                "aD",
                "Ec", "cc",
                "DE", "Ef",
            });
        }

        [Fact]
        public void Next_MultipleInvalidCharacters()
        {
            var caseSensitive = false;
            var alphabet = new Alphabet("ABCDEF");
            var scanner = new NgramScanner(2, caseSensitive, alphabet);

            var ngramValues1 = GetNgramValues("    AB   BCD   DE    ", scanner);

            ngramValues1.ShouldBe(new[]
            {
                "AB",
                "BC", "CD",
                "DE",
            });
        }

        [Fact]
        public void Next_WithoutClear()
        {
            var caseSensitive = false;
            var alphabet = new Alphabet("ABCDEF");
            var scanner = new NgramScanner(2, caseSensitive, alphabet);

            var ngramValues1 = GetNgramValues("ABCD", scanner);
            var ngramValues2 = GetNgramValues("CDEF", scanner);

            ngramValues1.ShouldBe(new[]
            {
                "AB", "BC", "CD"
            });
            ngramValues2.ShouldBe(new[]
            {
                "DC", "CD", "DE", "EF",
            });
        }

        [Fact]
        public void Next_WithClear()
        {
            var caseSensitive = false;
            var alphabet = new Alphabet("ABCDEF");
            var scanner = new NgramScanner(2, caseSensitive, alphabet);

            var ngramValues1 = GetNgramValues("ABCD", scanner);
            scanner.Clear();
            var ngramValues2 = GetNgramValues("CDEF", scanner);

            ngramValues1.ShouldBe(new[]
            {
                "AB", "BC", "CD"
            });
            ngramValues2.ShouldBe(new[]
            {
                "CD", "DE", "EF",
            });
        }

        private string[] GetNgramValues(string input, NgramScanner scanner)
        {
            return input
                .Select(character => scanner.Next(character, out var ngramValue) ? ngramValue : null)
                .Where(x => x != null)
                .Cast<string>()
                .ToArray();
        }
    }
}
