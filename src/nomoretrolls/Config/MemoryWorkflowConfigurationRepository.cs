using System.Collections.Concurrent;

namespace nomoretrolls.Config
{
    internal class MemoryWorkflowConfigurationRepository : IWorkflowConfigurationRepository
    {
        private readonly ConcurrentDictionary<string, WorkflowConfiguration> _cache;

        public MemoryWorkflowConfigurationRepository()
        {
            _cache = new ConcurrentDictionary<string, WorkflowConfiguration>(StringComparer.InvariantCultureIgnoreCase);
        }

        public Task<WorkflowConfiguration> GetWorkflowConfigAsync(string name)
        {
            _cache.TryGetValue(name, out WorkflowConfiguration config);
            
            return Task.FromResult(config);
        }

        public Task<IList<WorkflowConfiguration>> GetWorkflowConfigsAsync()
        {
            IList<WorkflowConfiguration> result = _cache.Values.ToList();

            return Task.FromResult(result);
        }

        public Task SetWorkflowConfigAsync(WorkflowConfiguration config)
        {
            _cache[config.Name] = config;

            return Task.CompletedTask;
        }
    }
}
