using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using nomoretrolls.Config;
using NSubstitute;
using Xunit;

namespace nomoretrolls.tests.Config
{
    public class WorkflowConfigurationRepositoryTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(3)]
        public async Task GetWorkflowConfigsAsync_CacheHydrated(int entryCount)
        {
            IList<WorkflowConfiguration>? entries = Enumerable.Range(0, entryCount)
                .Select(x => new WorkflowConfiguration()
                                { Name = $"config{x}", Enabled = true })
                .ToList();

            var cache = CreateCache();
            var persist = CreatePersistent();
            persist.GetWorkflowConfigsAsync().Returns(Task.FromResult(entries));

            var p = new WorkflowConfigurationRepository(cache, persist);

            var result = await p.GetWorkflowConfigsAsync();

            
            cache.Received(entryCount).SetWorkflowConfigAsync(Arg.Any<WorkflowConfiguration>());
            persist.Received(1).GetWorkflowConfigsAsync();
        }

        [Fact]
        public async Task GetWorkflowConfigAsync_CacheInvokedPersistIgnored()
        {
            var cache = CreateCache();
            var persist = CreatePersistent();
            
            var p = new WorkflowConfigurationRepository(cache, persist);

            var r = await p.GetWorkflowConfigAsync("test");

            cache.Received(1).GetWorkflowConfigAsync(Arg.Any<string>());
            persist.Received(0).GetWorkflowConfigAsync(Arg.Any<string>());
        }

        [Fact]
        public async Task GetWorkflowConfigAsync_CacheHitPersistIgnored()
        {
            var cache = CreateCache();
            cache.GetWorkflowConfigAsync(Arg.Any<string>())
                .Returns(Task.FromResult(new WorkflowConfiguration()));

            var persist = CreatePersistent();

            var p = new WorkflowConfigurationRepository(cache, persist);

            var r = await p.GetWorkflowConfigAsync("test");

            cache.Received(1).GetWorkflowConfigAsync(Arg.Any<string>());
            persist.Received(0).GetWorkflowConfigAsync(Arg.Any<string>());
        }


        [Fact]
        public async Task GetWorkflowConfigsAsync_CacheInvokedPersistIgnored()
        {
            var cache = CreateCache();
            var persist = CreatePersistent();

            var p = new WorkflowConfigurationRepository(cache, persist);

            var iterations = 3;
            for (var x = 1; x <= iterations; x++)
            {
                var r = await p.GetWorkflowConfigsAsync();
            }

            cache.Received(iterations).GetWorkflowConfigsAsync();
            persist.Received(1).GetWorkflowConfigsAsync();
        }


        [Fact]
        public async Task SetWorkflowConfigAsync_PersistAndCacheInvoked()
        {
            var cache = CreateCache();
            var persist = CreatePersistent();

            var p = new WorkflowConfigurationRepository(cache, persist);

            var config = new WorkflowConfiguration();

            await p.SetWorkflowConfigAsync(config);

            cache.Received(1).SetWorkflowConfigAsync(config);
            persist.Received(1).SetWorkflowConfigAsync(config);
        }

        private IWorkflowConfigurationRepository CreateCache() => Substitute.For<IWorkflowConfigurationRepository>();

        private IWorkflowConfigurationRepository CreatePersistent() => Substitute.For<IWorkflowConfigurationRepository>();
    }
}
