using nomoretrolls.Config;

namespace nomoretrolls.Workflows.Parts
{
    internal class WorkflowConfigEnabled : IMessageWorkflowPart
    {
        private readonly IWorkflowConfigurationRepository _configRepo;
        private readonly string _name;

        public WorkflowConfigEnabled(Config.IWorkflowConfigurationRepository configRepo, string name)
        {
            _configRepo = configRepo;
            _name = name;
        }

        public async Task<MessageWorkflowContext?> ExecuteAsync(MessageWorkflowContext context)
        {
            var config = await _configRepo.GetWorkflowConfigAsync(_name);

            var enabled = config != null ? config.Enabled : true;
            
            return enabled ? context : null;
        }
    }
}
