using Rekog.Core;
using Rekog.Core.Ngram;
using System;
using System.Linq;
using Xunit;

namespace Rekog.UnitTests.Core.Ngrams
{
    public class NgramAnalyzerBufferTests
    {
        [Fact]
        public void Ctor_SizeLessThan1()
        {
            var alphabet = new Alphabet("ABCDEF");

            Assert.Throws<ArgumentOutOfRangeException>(() => new NgramAnalyzerBuffer(0, false, alphabet));
        }

        [Fact]
        public void Next_Size1()
        {
            var caseSensitive = false;
            var alphabet = new Alphabet("ABCDEF", caseSensitive);
            var buffer = new NgramAnalyzerBuffer(1, caseSensitive, alphabet);

            var ngramValues = GetNgramValues("AB CDEF EAF AZBC", buffer);

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
            var alphabet = new Alphabet("ABCDEF", caseSensitive);
            var buffer = new NgramAnalyzerBuffer(2, caseSensitive, alphabet);

            var ngramValues = GetNgramValues("AB CDEF EAF AZBC", buffer);

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
            var alphabet = new Alphabet("ABCDEF", caseSensitive);
            var buffer = new NgramAnalyzerBuffer(4, caseSensitive, alphabet);

            var ngramValues = GetNgramValues("AB CDEF EAFAB AZBC ABC", buffer);

            Assert.Equal(new[]
            {
                "CDEF",
                "EAFA", "AFAB",
            }, ngramValues);
        }

        [Fact]
        public void Next_CaseSensitive()
        {
            var caseSensitive = true;
            var alphabet = new Alphabet("abcDEF", caseSensitive);
            var buffer = new NgramAnalyzerBuffer(2, caseSensitive, alphabet);

            var ngramValues = GetNgramValues("aB abc aD Ecc DEf", buffer);

            Assert.Equal(new[]
            {
                "ab", "bc",
                "aD",
                "Ec", "cc",
                "DE",
            }, ngramValues);
        }

        [Fact]
        public void Next_MultipleInvalidCharacters()
        {
            var caseSensitive = false;
            var alphabet = new Alphabet("ABCDEF", caseSensitive);
            var buffer = new NgramAnalyzerBuffer(2, caseSensitive, alphabet);

            var ngramValues1 = GetNgramValues("    AB   BCD   DE    ", buffer);

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
            var alphabet = new Alphabet("ABCDEF", caseSensitive);
            var buffer = new NgramAnalyzerBuffer(2, caseSensitive, alphabet);

            var ngramValues1 = GetNgramValues("ABCD", buffer);
            var ngramValues2 = GetNgramValues("CDEF", buffer);

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
            var alphabet = new Alphabet("ABCDEF", caseSensitive);
            var buffer = new NgramAnalyzerBuffer(2, caseSensitive, alphabet);

            var ngramValues1 = GetNgramValues("ABCD", buffer);
            buffer.Clear();
            var ngramValues2 = GetNgramValues("CDEF", buffer);

            Assert.Equal(new[]
            {
                "AB", "BC", "CD"
            }, ngramValues1);
            Assert.Equal(new[]
            {
                "CD", "DE", "EF",
            }, ngramValues2);
        }

        private string[] GetNgramValues(string input, NgramAnalyzerBuffer buffer)
        {
            return input
                .Select(character => buffer.Next(character, out var ngramValue) ? ngramValue : null)
                .Where(x => x != null)
                .Cast<string>()
                .ToArray();
        }
    }
}
