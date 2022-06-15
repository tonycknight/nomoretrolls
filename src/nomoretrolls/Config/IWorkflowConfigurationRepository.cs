namespace nomoretrolls.Config
{
    internal interface IWorkflowConfigurationRepository
    {
        public const string ShoutingWorkflow = "shouting";
        public const string BlacklistWorkflow = "blacklist";
        public const string EmoteAnnotationWorkflow = "emotes";

        public Task<IList<WorkflowConfiguration>> GetWorkflowConfigsAsync();

        public Task<WorkflowConfiguration> GetWorkflowConfigAsync(string name);

        public Task SetWorkflowConfigAsync(WorkflowConfiguration config);
    }
}
