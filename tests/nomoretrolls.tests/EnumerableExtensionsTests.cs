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
    }
}
