using System;
using FluentAssertions;
using nomoretrolls.Workflows;
using NSubstitute;
using Xunit;

namespace nomoretrolls.tests.Workflows
{
    public class MessageWorkflowFactoryTests
    {
        [Fact]
        public void CreateBuilder_ReturnsBuilder()
        {
            var sp = Substitute.For<IServiceProvider>();

            var fact = new MessageWorkflowFactory(sp);

            var result = fact.CreateBuilder();

            result.Should().NotBeNull();
        }

    }
}
