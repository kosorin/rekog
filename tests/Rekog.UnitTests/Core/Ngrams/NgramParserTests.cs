using Rekog.Core;
using Rekog.Core.Ngram;
using System;
using System.Linq;
using Xunit;

namespace Rekog.UnitTests.Core.Ngrams
{
    public class NgramParserTests
    {
        [Fact]
        public void Ctor_SizeLessThan1()
        {
            var alphabet = new Alphabet("ABCDEF");

            Assert.Throws<ArgumentOutOfRangeException>(() => new NgramParser(0, false, alphabet));
        }

        [Fact]
        public void Next_Size1()
        {
            var caseSensitive = false;
            var alphabet = new Alphabet("ABCDEF");
            var parser = new NgramParser(1, caseSensitive, alphabet);

            var ngramValues = GetNgramValues("AB CDEF EAF AZBC", parser);

            Assert.Equal(new[]
            {
                "A", "B",
                "C", "D", "E", "F",
                "E", "A", "F",
                "A", "B", "C",
            }, ngramValues);
        }

        [Fact]
        public void Next_Size2()
        {
            var caseSensitive = false;
            var alphabet = new Alphabet("ABCDEF");
            var parser = new NgramParser(2, caseSensitive, alphabet);

            var ngramValues = GetNgramValues("AB CDEF EAF AZBC", parser);

            Assert.Equal(new[]
            {
                "AB",
                "CD", "DE", "EF",
                "EA", "AF",
                "BC",
            }, ngramValues);
        }

        [Fact]
        public void Next_Size4()
        {
            var caseSensitive = false;
            var alphabet = new Alphabet("ABCDEF");
            var parser = new NgramParser(4, caseSensitive, alphabet);

            var ngramValues = GetNgramValues("AB CDEF EAFAB AZBC ABC", parser);

            Assert.Equal(new[]
            {
                "CDEF",
                "EAFA", "AFAB",
            }, ngramValues);
        }

        [Fact]
        public void Next_CaseInsensitive()
        {
            var caseSensitive = false;
            var alphabet = new Alphabet("ABCDEF");
            var parser = new NgramParser(2, caseSensitive, alphabet);

            var ngramValues = GetNgramValues("aB abc aD Ecc DEf", parser);

            Assert.Equal(new[]
            {
                "AB",
                "AB", "BC",
                "AD",
                "EC", "CC",
                "DE", "EF",
            }, ngramValues);
        }

        [Fact]
        public void Next_CaseSensitive()
        {
            var caseSensitive = true;
            var alphabet = new Alphabet("ABCDEF");
            var parser = new NgramParser(2, caseSensitive, alphabet);

            var ngramValues = GetNgramValues("aB abc aD Ecc DEf", parser);

            Assert.Equal(new[]
            {
                "aB",
                "ab", "bc",
                "aD",
                "Ec", "cc",
                "DE", "Ef",
            }, ngramValues);
        }

        [Fact]
        public void Next_MultipleInvalidCharacters()
        {
            var caseSensitive = false;
            var alphabet = new Alphabet("ABCDEF");
            var parser = new NgramParser(2, caseSensitive, alphabet);

            var ngramValues1 = GetNgramValues("    AB   BCD   DE    ", parser);

            Assert.Equal(new[]
            {
                "AB",
                "BC", "CD",
                "DE",
            }, ngramValues1);
        }

        [Fact]
        public void Next_WithoutClear()
        {
            var caseSensitive = false;
            var alphabet = new Alphabet("ABCDEF");
            var parser = new NgramParser(2, caseSensitive, alphabet);

            var ngramValues1 = GetNgramValues("ABCD", parser);
            var ngramValues2 = GetNgramValues("CDEF", parser);

            Assert.Equal(new[]
            {
                "AB", "BC", "CD"
            }, ngramValues1);
            Assert.Equal(new[]
            {
                "DC", "CD", "DE", "EF",
            }, ngramValues2);
        }

        [Fact]
        public void Next_WithClear()
        {
            var caseSensitive = false;
            var alphabet = new Alphabet("ABCDEF");
            var parser = new NgramParser(2, caseSensitive, alphabet);

            var ngramValues1 = GetNgramValues("ABCD", parser);
            parser.Clear();
            var ngramValues2 = GetNgramValues("CDEF", parser);

            Assert.Equal(new[]
            {
                "AB", "BC", "CD"
            }, ngramValues1);
            Assert.Equal(new[]
            {
                "CD", "DE", "EF",
            }, ngramValues2);
        }

        private string[] GetNgramValues(string input, NgramParser parser)
        {
            return input
                .Select(character => parser.Next(character, out var ngramValue) ? ngramValue : null)
                .Where(x => x != null)
                .Cast<string>()
                .ToArray();
        }
    }
}
