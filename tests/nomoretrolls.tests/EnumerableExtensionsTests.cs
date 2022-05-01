using FluentAssertions;
using Xunit;

namespace nomoretrolls.tests
{
    public class EnumerableExtensionsTests
    {

        [Fact]
        public void Entropy_ReturnsZeroForEmptySet()
        {
            var values = new char[0];
            var r = values.Entropy();

            r.Should().Be(0.0);
        }

        [Theory]
        [InlineData('a')]
        [InlineData('a', 'a', 'a')]
        [InlineData('b', 'b', 'b', 'b', 'b', 'b', 'b', 'b')]
        public void Entropy_ReturnsZeroForSet(params char[] values)
        {
            var r = values.Entropy();

            r.Should().Be(0.0);
        }

        [Theory]
        [InlineData(1.0, 'a', 'b')]
        [InlineData(1.0, 'a', 'b', 'a', 'b')]
        [InlineData(1.0, 'a', 'b', 'a', 'b', 'a', 'b', 'a', 'b', 'a', 'b', 'a', 'b')]
        public void Entropy_ReturnsForEqualSplitSet(double expected, params char[] values)
        {
            var r = values.Entropy();

            r.Should().Be(expected);
        }

        [Theory]
        [InlineData(1.584962500721156, 'a', 'b', 'c')]
        [InlineData(1.584962500721156, 'a', 'b', 'c', 'a', 'b', 'c')]
        [InlineData(1.584962500721156, 'a', 'b', 'c', 'a', 'b', 'c', 'a', 'b', 'c', 'a', 'b', 'c', 'a', 'b', 'c', 'a', 'b', 'c')]
        public void Entropy_ReturnsForNonEqualSplitSet(double expected, params char[] values)
        {
            var r = values.Entropy();

            r.Should().Be(expected);
        }

        [Theory]
        [InlineData(0.9182958340544896, 'a', 'a', 'c')]
        [InlineData(0.7219280948873623, 'a', 'a', 'a', 'a', 'c')]
        [InlineData(0.9182958340544896, 'c', 'a', 'a', 'a', 'a', 'c')]
        [InlineData(0.6500224216483541, 'a', 'a', 'a', 'a', 'a', 'c')]
        [InlineData(0.5916727785823275, 'a', 'a', 'a', 'a', 'a', 'a', 'c')]
        [InlineData(0.5435644431995964, 'a', 'a', 'a', 'a', 'a', 'a', 'c', 'a')]
        [InlineData(0.3095434291503252, 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'c')]
        public void Entropy_ReturnsForTerminatingSet(double expected, params char[] values)
        {
            var r = values.Entropy();

            r.Should().Be(expected);
        }

        //

        [Fact]
        public void GiniImpurity_ReturnsZeroForEmptySet()
        {
            var values = new char[0];
            var r = values.GiniImpurity();

            r.Should().Be(0.0);
        }

        [Theory]
        [InlineData('a')]
        [InlineData('a', 'a', 'a')]
        [InlineData('b', 'b', 'b', 'b', 'b', 'b', 'b', 'b')]
        public void GiniImpurity_ReturnsZeroForSet(params char[] values)
        {
            var r = values.GiniImpurity();

            r.Should().Be(0.0);
        }

        [Theory]
        [InlineData('a', 'b')]
        [InlineData('a', 'b', 'a', 'b')]
        [InlineData('a', 'b', 'a', 'b', 'a', 'b', 'a', 'b', 'a', 'b', 'a', 'b')]
        public void GiniImpurity_ReturnsForEqualSplitSet(params char[] values)
        {
            var r = values.GiniImpurity();

            r.Should().Be(0.5);
        }

        [Theory]
        [InlineData(0.6666666666666667, 'a', 'b', 'c')]
        [InlineData(0.6666666666666667, 'a', 'b', 'c', 'a', 'b', 'c')]
        [InlineData(0.6666666666666667, 'a', 'b', 'c', 'a', 'b', 'c', 'a', 'b', 'c', 'a', 'b', 'c', 'a', 'b', 'c', 'a', 'b', 'c')]
        public void GiniImpurity_ReturnsForNonEqualSplitSet(double expected, params char[] values)
        {
            var r = values.GiniImpurity();

            r.Should().Be(expected);
        }

        [Theory]
        [InlineData(0.4444444444444444, 'a', 'a', 'c')]
        [InlineData(0.32000000000000006, 'a', 'a', 'a', 'a', 'c')]
        [InlineData(0.48, 'c', 'a', 'a', 'a', 'c')]
        [InlineData(0.2777777777777778, 'a', 'a', 'a', 'a', 'a', 'c')]
        [InlineData(0.4444444444444444, 'c', 'a', 'a', 'a', 'a', 'c')]
        [InlineData(0.24489795918367344, 'a', 'a', 'a', 'a', 'a', 'a', 'c')]
        [InlineData(0.21875, 'a', 'a', 'a', 'a', 'a', 'a', 'c', 'a')]
        [InlineData(0.10493827160493827, 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'a', 'c')]
        public void GiniImpurity_ReturnsForTerminatingSet(double expected, params char[] values)
        {
            var r = values.GiniImpurity();

            r.Should().Be(expected);
        }
    }
}
