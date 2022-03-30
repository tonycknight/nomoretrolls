namespace nomoretrolls.Config
{
    internal class WorkflowConfigurationRepository : IWorkflowConfigurationRepository
    {
        private readonly Lazy<IWorkflowConfigurationRepository> _cache;
        private readonly IWorkflowConfigurationRepository _persistent;

        public WorkflowConfigurationRepository(IWorkflowConfigurationRepository cache, IWorkflowConfigurationRepository persistent)
        {
            _cache = new Lazy<IWorkflowConfigurationRepository>(() => HydrateCache(cache, persistent).GetAwaiter().GetResult());
            _persistent = persistent;
        }

        public Task<WorkflowConfiguration> GetWorkflowConfigAsync(string name)
            => _cache.Value.GetWorkflowConfigAsync(name);

        public Task<IList<WorkflowConfiguration>> GetWorkflowConfigsAsync()
            => _cache.Value.GetWorkflowConfigsAsync();

        public async Task SetWorkflowConfigAsync(WorkflowConfiguration config)
        {
            await _persistent.SetWorkflowConfigAsync(config);
            await _cache.Value.SetWorkflowConfigAsync(config);
        }

        private static async Task<IWorkflowConfigurationRepository> HydrateCache(IWorkflowConfigurationRepository cache, IWorkflowConfigurationRepository persistent)
        {
            var configs = await persistent.GetWorkflowConfigsAsync();

            foreach(var c in configs)
            {
                await cache.SetWorkflowConfigAsync(c);
            }

            return cache;
        }
    }
}
