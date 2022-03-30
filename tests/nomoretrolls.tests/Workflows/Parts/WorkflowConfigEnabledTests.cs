using System.Threading.Tasks;
using FluentAssertions;
using nomoretrolls.Messaging;
using nomoretrolls.Workflows;
using nomoretrolls.Workflows.Parts;
using NSubstitute;
using Xunit;

namespace nomoretrolls.tests.Workflows.Parts
{
    public class WorkflowConfigEnabledTests
    {
        [Fact]
        public async Task ExecuteAsync_RepoQueried_ReturnsContext()
        {
            var name = "test";
            var config = new nomoretrolls.Config.WorkflowConfiguration() { Name = name, Enabled = true };
            var messageContext = Substitute.For<IDiscordMessageContext>();
            var repo = Substitute.For<nomoretrolls.Config.IWorkflowConfigurationRepository>();
            repo.GetWorkflowConfigAsync(name).Returns(Task.FromResult(config));

            var context = new MessageWorkflowContext(messageContext);

            var part = new WorkflowConfigEnabled(repo, name);

            var result = await part.ExecuteAsync(context);

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task ExecuteAsync_RepoQueried_DoesNotReturnContext()
        {
            var name = "test";
            var config = new nomoretrolls.Config.WorkflowConfiguration() { Name = name, Enabled = false };
            var messageContext = Substitute.For<IDiscordMessageContext>();
            var repo = Substitute.For<nomoretrolls.Config.IWorkflowConfigurationRepository>();
            repo.GetWorkflowConfigAsync(name).Returns(Task.FromResult(config));

            var context = new MessageWorkflowContext(messageContext);

            var part = new WorkflowConfigEnabled(repo, name);

            var result = await part.ExecuteAsync(context);

            result.Should().BeNull();
        }

        [Fact]
        public async Task ExecuteAsync_RepoQueried_DefaultReturned()
        {
            var name = "test";
            var messageContext = Substitute.For<IDiscordMessageContext>();
            var repo = Substitute.For<nomoretrolls.Config.IWorkflowConfigurationRepository>();
            nomoretrolls.Config.WorkflowConfiguration config = null;
            repo.GetWorkflowConfigAsync(name).Returns(Task.FromResult(config));

            var context = new MessageWorkflowContext(messageContext);

            var part = new WorkflowConfigEnabled(repo, name);

            var result = await part.ExecuteAsync(context);

            result.Should().NotBeNull();
        }
    }
}
