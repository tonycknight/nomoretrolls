namespace nomoretrolls.Config
{
    internal class WorkflowConfigurationRepository : IWorkflowConfigurationRepository
    {
        private readonly Lazy<Task<IWorkflowConfigurationRepository>> _cache;
        private readonly IWorkflowConfigurationRepository _persistent;

        public WorkflowConfigurationRepository(IWorkflowConfigurationRepository cache, IWorkflowConfigurationRepository persistent)
        {
            _cache = new Lazy<Task<IWorkflowConfigurationRepository>>(async () => await HydrateCache(cache, persistent));
            _persistent = persistent;
        }

        public async Task<WorkflowConfiguration> GetWorkflowConfigAsync(string name)
        {
            var cache = await _cache.Value;
            return await cache.GetWorkflowConfigAsync(name);
        }

        public async Task<IList<WorkflowConfiguration>> GetWorkflowConfigsAsync()
        {
            var cache = await _cache.Value;
            return await cache.GetWorkflowConfigsAsync();
        }

        public async Task SetWorkflowConfigAsync(WorkflowConfiguration config)
        {
            await _persistent.SetWorkflowConfigAsync(config);
            var cache = await _cache.Value;
            await cache.SetWorkflowConfigAsync(config);
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
