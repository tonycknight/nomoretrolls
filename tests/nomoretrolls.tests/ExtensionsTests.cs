using FluentAssertions;
using Xunit;

namespace nomoretrolls.tests
{
    public class ExtensionsTests
    {
        [Theory]
        [InlineData(true, 0)]
        [InlineData(false, 2)]
        public void ToReturnCode_ExpectationReturned(bool value, int expected)
        {
            var result = value.ToReturnCode();

            result.Should().Be(expected);
        }
    }
}
