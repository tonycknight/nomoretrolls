namespace nomoretrolls.Config
{
    internal interface IWorkflowConfigurationRepository
    {
        public const string CapitalsWorkflow = "capitals";
        public const string BlacklistWorkflow = "blacklist";

        public Task<IList<WorkflowConfiguration>> GetWorkflowConfigsAsync();

        public Task<WorkflowConfiguration> GetWorkflowConfigAsync(string name);

        public Task SetWorkflowConfigAsync(WorkflowConfiguration config);
    }
}
