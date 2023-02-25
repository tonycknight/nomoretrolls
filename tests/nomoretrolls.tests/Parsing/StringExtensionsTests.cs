using System;
using System.Linq;
using FluentAssertions;
using nomoretrolls.Parsing;
using Xunit;

namespace nomoretrolls.tests.Parsing
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData("", false)]
        [InlineData(" ", false)]
        [InlineData("a", false)]
        [InlineData(" a ", false)]
        [InlineData("A", true)]
        [InlineData(" A ", false)]
        [InlineData("Aa", false)]
        [InlineData("AaA", false)]
        [InlineData("AAA", true)]
        [InlineData("A A", false)]
        public void IsAllCapitals_ReturnsExpected(string value, bool expected)
        {
            value.IsAllCapitals().Should().Be(expected);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("!!!!")]
        [InlineData("a b c", "a", "b", "c")]
        [InlineData("a B c D", "a", "B", "c", "D")]
        [InlineData("  a   B   c   D  ", "a", "B", "c", "D")]
        [InlineData("  12   B   34   D  56  ", "12", "B", "34", "D", "56")]
        [InlineData("aB!", "aB")]
        [InlineData("!aB!", "aB")]
        public void WordSplit_StringIsSplit(string value, params string[] expected)
        {
            var words = value.SplitWords().ToArray();

            words.SequenceEqual(expected).Should().BeTrue();
        }


        [Theory]
        [InlineData("", 0, 0, 0)]
        [InlineData(" ", 0, 0, 0)]
        [InlineData("aaa", 1, 0, 3)]
        [InlineData("A BEE", 2, 2, 3)]
        [InlineData(" a BEE ", 2, 1, 3)]
        [InlineData(" a BEE HIVE", 3, 2, 4)]
        public void WordCapitalsSpread_WordsAnalysed(string value, int expectedWordCount, int expectedCapitalsCount, int expectedMaxLength)
        {
            var (wordCount, capitalsCount, maxWordLength) = value.SplitWords().WordCapitalsSpread();

            wordCount.Should().Be(expectedWordCount);
            capitalsCount.Should().Be(expectedCapitalsCount);
            maxWordLength.Should().Be(expectedMaxLength);
        }

        [Theory]
        [InlineData("", null, null)]
        [InlineData("aaa", null, null)]
        [InlineData("aaa#", null, null)]
        [InlineData("aaa##", "aaa", "#")]
        [InlineData("aaa## ", "aaa", "# ")]
        [InlineData("aaa#1234", "aaa", "1234")]
        [InlineData("aaa##1234", "aaa", "#1234")]
        [InlineData(" aaa##1234 ", " aaa", "#1234 ")]
        [InlineData("#1234", null, null)]
        [InlineData("##1234", null, null)]
        [InlineData("#", null, null)]
        [InlineData("##", null, null)]
        [InlineData(" # ", null, null)]
        public void DeconstructDiscordName_NameSplit(string value, string expectedName, string expectedDiscriminator)
        {
            var (un, d) = value.DeconstructDiscordName();

            un.Should().Be(expectedName);
            d.Should().Be(expectedDiscriminator);
        }


        [Theory]
        [InlineData("", 0.0)]
        [InlineData(" ", 0.0)]
        [InlineData(" a ", 0.0)]
        [InlineData(" aA ", 0.5)]
        [InlineData(" Aa ", 0.5)]
        [InlineData(" A ", 1.0)]
        [InlineData(" AB ", 1.0)]
        [InlineData(" TESTING TESTING TESTing ", 0.8571428571428571)]
        [InlineData(" TeStInG tEsTiNg TeStInG ", 0.5238095238095238)]
        [InlineData(" TeStInG tEsTiNg TeStiNg ", 0.47619047619047616)]
        [InlineData(" TeSt TeSt ", 0.5)]
        public void AnalyseCapitals_CapitalRatio(string value, double expected)
        {
            var r = value.AnalyseCapitals();

            r.CapitalRatio.Should().Be(expected);
        }


    }
}
