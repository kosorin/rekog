using System.Linq;
using System.Text;
using Rekog.Core.Corpora;
using Serilog;
using Serilog.Sinks.TestCorrelator;
using Shouldly;
using Xunit;

namespace Rekog.Core.UnitTests.Corpora
{
    public class CharacterReaderTests
    {
        private readonly ILogger _readerLogger;

        public CharacterReaderTests()
        {
            _readerLogger = new LoggerConfiguration()
                .WriteTo.TestCorrelator()
                .CreateLogger()
                .ForContext<CharacterReader>();
        }

        [Fact]
        public void ReadNext()
        {
            var reader = new CharacterReader(_readerLogger);

            var result1 = reader.ReadNext('a', out var character1);
            var result2 = reader.ReadNext('b', out var character2);
            var result3 = reader.ReadNext('c', out var character3);

            result1.ShouldBeTrue();
            result2.ShouldBeTrue();
            result3.ShouldBeTrue();

            character1.ShouldBe(new Rune('a'));
            character2.ShouldBe(new Rune('b'));
            character3.ShouldBe(new Rune('c'));
        }

        [Fact]
        public void ReadNext_UnicodeCharacters()
        {
            var reader = new CharacterReader(_readerLogger);

            var result1 = reader.ReadNext((char)0xD83C, out _);
            var result2 = reader.ReadNext((char)0xDF04, out var character2);
            var result3 = reader.ReadNext('x', out var character3);

            result1.ShouldBeFalse();
            result2.ShouldBeTrue();
            result3.ShouldBeTrue();

            character2.ShouldBe(new Rune((char)0xD83C, (char)0xDF04));
            character3.ShouldBe(new Rune('x'));
        }

        [Fact]
        public void ReadNext_UnicodeCharacters_MultipleHighSurrogate()
        {
            var reader = new CharacterReader(_readerLogger);

            var result1 = reader.ReadNext((char)0xD83C, out _);

            bool result2;
            using (TestCorrelator.CreateContext())
            {
                result2 = reader.ReadNext((char)0xD83C, out _);

                var events = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
                events.Count.ShouldBe(1);
                events.First().MessageTemplate.Text.ShouldBe("Multiple high surrogates.");
            }

            var result3 = reader.ReadNext((char)0xDF04, out var character3);
            var result4 = reader.ReadNext('x', out var character4);

            result1.ShouldBeFalse();
            result2.ShouldBeFalse();
            result3.ShouldBeTrue();
            result4.ShouldBeTrue();

            character3.ShouldBe(new Rune((char)0xD83C, (char)0xDF04));
            character4.ShouldBe(new Rune('x'));
        }

        [Fact]
        public void ReadNext_UnicodeCharacters_MissingHighSurrogate()
        {
            var reader = new CharacterReader(_readerLogger);

            bool result1;
            using (TestCorrelator.CreateContext())
            {
                result1 = reader.ReadNext((char)0xDF04, out _);

                var events = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
                events.Count.ShouldBe(1);
                events.First().MessageTemplate.Text.ShouldBe("Missing high surrogate.");
            }

            var result2 = reader.ReadNext('x', out var character2);

            result1.ShouldBeFalse();
            result2.ShouldBeTrue();

            character2.ShouldBe(new Rune('x'));
        }

        [Fact]
        public void ReadNext_UnicodeCharacters_MissingLowSurrogate()
        {
            var reader = new CharacterReader(_readerLogger);

            var result1 = reader.ReadNext((char)0xD83C, out _);

            bool result2;
            Rune character2;
            using (TestCorrelator.CreateContext())
            {
                result2 = reader.ReadNext('x', out character2);

                var events = TestCorrelator.GetLogEventsFromCurrentContext().ToList();
                events.Count.ShouldBe(1);
                events.First().MessageTemplate.Text.ShouldBe("Missing low surrogate.");
            }

            result1.ShouldBeFalse();
            result2.ShouldBeTrue();

            character2.ShouldBe(new Rune('x'));
        }
    }
}
