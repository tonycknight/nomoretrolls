using FluentAssertions;
using nomoretrolls.Commands;
using nomoretrolls.Workflows;
using NSubstitute;
using Xunit;


namespace nomoretrolls.tests.Workflows
{
    public class MessageWorkflowProviderTests
    {
        [Fact]
        public void CreateBlacklistedUserWorkflow_ResultReturned()
        {
            var wff = Substitute.For<IMessageWorkflowFactory>();

            var mwp = new MessageWorkflowProvider(wff);

            var r = mwp.CreateBlacklistedUserWorkflow();

            r.Should().NotBeNull();
        }

        [Fact]
        public void CreateBlacklistedPersonalReplyWorkflow_ResultsReturned()
        {
            var wff = Substitute.For<IMessageWorkflowFactory>();

            var mwp = new MessageWorkflowProvider(wff);

            var r = mwp.CreateBlacklistedPersonalReplyWorkflow();

            r.Should().NotBeNull();
        }

        [Fact]
        public void CreateShoutingWorkflow_ResultsReturned()
        {
            var wff = Substitute.For<IMessageWorkflowFactory>();

            var mwp = new MessageWorkflowProvider(wff);

            var r = mwp.CreateShoutingWorkflow();

            r.Should().NotBeNull();
        }


        [Fact]
        public void CreateShoutingPersonalReplyWorkflow_ResultsReturned()
        {
            var wff = Substitute.For<IMessageWorkflowFactory>();

            var mwp = new MessageWorkflowProvider(wff);

            var r = mwp.CreateShoutingPersonalReplyWorkflow();

            r.Should().NotBeNull();
        }

        [Fact]
        public void CreateAutoEmoteWorkflow_ResultsReturned()
        {
            var wff = Substitute.For<IMessageWorkflowFactory>();

            var mwp = new MessageWorkflowProvider(wff);

            var r = mwp.CreateAutoEmoteWorkflow();

            r.Should().NotBeNull();
        }
    }
}
