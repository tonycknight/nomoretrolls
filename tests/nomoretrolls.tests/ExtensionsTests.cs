using System;
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

        [Theory]
        [InlineData(null)]
        public void ArgNotNull_ExceptionThrown(string value)
        {
            var f = () => value.ArgNotNull(nameof(value));

            f.Should().Throw<ArgumentNullException>().WithParameterName(nameof(value));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void ArgNotNull_ExceptionNotThrown(string value)
        {
            var result = value.ArgNotNull(nameof(value));

            result.Should().Be(value);            
        }

        [Fact]
        public void InvalidOpArg_NullValue_ExceptionThrown()
        {
            string? value = null;

            var f = () => value.InvalidOpArg(s => s != null, "test");

            f.Should().Throw<InvalidOperationException>().WithMessage("?*");
        }

        [Fact]
        public void InvalidOpArg_NullPredicate_ExceptionThrown()
        {
            string value = "";

            var f = () => value.InvalidOpArg(null, "test");

            f.Should().Throw<ArgumentNullException>().WithMessage("?*");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(" a ")]
        public void InvalidOpArg_InvalidValue_ExceptionThrown(string value)
        {

            var f = () => value.InvalidOpArg(s => s != "aaa", "test");

            f.Should().Throw<InvalidOperationException>().WithMessage("?*");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(" a ")]
        public void InvalidOpArg_ValidValue_ExceptionNotThrown(string value)
        {

            var result = value.InvalidOpArg(s => false, "test");

            result.Should().Be(value);
        }

        [Fact]
        public void Pipe_NullSelector_ExceptionThrown()
        {
            Func<string, int> s = null;

            var f = () => "".Pipe(s);

            f.Should().Throw<ArgumentNullException>().WithMessage("?*");
        }

        [Theory]
        [InlineData("", 0)]
        [InlineData("a", 1)]
        [InlineData("abc", 3)]
        public void Pipe_SelectorApplied(string value, int expected)
        {
            var result = value.Pipe(s => s.Length);

            result.Should().Be(expected);
        }

    }
}
