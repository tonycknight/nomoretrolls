using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using nomoretrolls.Config;
using Xunit;

namespace nomoretrolls.tests.Config
{
    public class MemoryWorkflowConfigurationRepositoryTests
    {
        [Fact]
        public async Task SetWorkflowAsync_NewEntry()
        {
            var p = new MemoryWorkflowConfigurationRepository();

            var config = new WorkflowConfiguration() { Name = "test" };

            await p.SetWorkflowConfigAsync(config);

            var r = await p.GetWorkflowConfigAsync(config.Name);

            r.Should().Be(config);
        }

        [Fact]
        public async Task SetWorkflowAsync_UpdatedEntry()
        {
            var p = new MemoryWorkflowConfigurationRepository();

            var config1 = new WorkflowConfiguration() { Name = "test", Enabled = false };
            var config2 = new WorkflowConfiguration() { Name = config1.Name, Enabled = true };

            await p.SetWorkflowConfigAsync(config1);

            await p.SetWorkflowConfigAsync(config2);

            var r = await p.GetWorkflowConfigAsync(config1.Name);

            r.Should().Be(config2);
        }

        [Fact]
        public async Task GetWorkflowConfigsAsync_ReturnsAll()
        {
            var configs = Enumerable.Range(1, 3)
                                    .Select(i => new WorkflowConfiguration() { Name = $"config{i}", Enabled = true })
                                    .ToList();

            var p = new MemoryWorkflowConfigurationRepository();

            foreach (var c in configs)
            {
                await p.SetWorkflowConfigAsync(c);
            }

            var r = await p.GetWorkflowConfigsAsync();

            r.Should().BeEquivalentTo(configs);
        }


    }
}
