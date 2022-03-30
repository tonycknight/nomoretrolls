using FluentAssertions;
using nomoretrolls.Formatting;
using Xunit;

namespace nomoretrolls.tests.Formatting
{
    public class MarkdownExtensionsTests
    {
        [Theory]
        [InlineData("", "````")]
        [InlineData(" ", "`` ``")]
        [InlineData("a", "``a``")]
        [InlineData("`a", "```a``")]
        [InlineData("a`", "``a```")]
        [InlineData("`a`", "```a```")]
        [InlineData("B a", "``B a``")]
        [InlineData("``a", "``a")]
        [InlineData("a``", "a``")]
        public void ToCode_Formats(string value, string expectedValue)
        {
            var r = value.ToCode();
            r.Should().Be(expectedValue);
        }
    }
}
