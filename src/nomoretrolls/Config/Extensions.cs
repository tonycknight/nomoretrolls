namespace nomoretrolls.Config
{
    internal static class Extensions
    {
        public static WorkflowConfigurationDto ToDto(this WorkflowConfiguration value)
            => new WorkflowConfigurationDto()
            {
                Name = value.Name,
                Enabled = value.Enabled,
            };

        public static WorkflowConfiguration FromDto(this WorkflowConfigurationDto value)
            => new WorkflowConfiguration()
            {
                Name = value.Name,
                Enabled = value.Enabled,
            };
    }
}
