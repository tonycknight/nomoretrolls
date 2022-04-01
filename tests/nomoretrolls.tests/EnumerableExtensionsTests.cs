using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using Xunit;

namespace nomoretrolls.tests
{
    public class EnumerableExtensionsTests
    {
        [Fact]
        public void TryGet_NullValues_ExceptionThrown()
        {
            Dictionary<string, string>? d = null;

            var f = () => d.TryGet("a");

            f.Should().Throw<ArgumentNullException>().WithMessage("?*");
        }

        [Fact]
        public void TryGet_NullKey_ExceptionThrown()
        {
            var d = new Dictionary<string, string>();

            var f = () => d.TryGet(null);

            f.Should().Throw<ArgumentNullException>().WithMessage("?*");
        }

        [Property(Verbose = true)]
        public bool TryGet_KeyFound_ValueReturned(PositiveInt value)
        {
            var keys = Enumerable.Range(value.Get, 3)
                                 .Select(i => i.ToString())
                                 .ToList();

            var dict = keys.ToDictionary(k => k, k => k + k);

            var key = keys.Last();

            return dict.TryGet(key) == key + key;
        }

        [Property(Verbose = true)]
        public bool TryGet_KeyNotFound_NullReturned(PositiveInt value)
        {
            var keys = Enumerable.Range(value.Get, 3)
                                 .Select(i => i.ToString())
                                 .ToList();

            var dict = keys.Take(1).ToDictionary(k => k, k => k + k);

            var key = keys.Last();

            return dict.TryGet(key) == null;
        }

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
