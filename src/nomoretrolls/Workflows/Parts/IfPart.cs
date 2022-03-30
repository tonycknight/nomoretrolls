using nomoretrolls.Telemetry;

namespace nomoretrolls.Workflows.Parts
{
    internal class IfPart : IMessageWorkflowPart
    {
        private readonly ITelemetry _telemetry;
        private readonly IMessageWorkflowExecutor _exec;

        public IfPart(ITelemetry telemetry, 
                    IMessageWorkflowExecutor exec,    
                    IMessageWorkflowPart condition, 
                    IMessageWorkflow onSuccess, 
                    IMessageWorkflow onFailure)
        {
            _telemetry = telemetry.ArgNotNull(nameof(telemetry));
            _exec = exec.ArgNotNull(nameof(exec));
            Condition = condition.ArgNotNull(nameof(condition));
            OnSuccess = onSuccess.ArgNotNull(nameof(onSuccess));
            OnFailure = onFailure.ArgNotNull(nameof(onFailure));
        }

        public IMessageWorkflowPart Condition { get; }
        public IMessageWorkflow OnSuccess { get; }
        public IMessageWorkflow OnFailure { get; }

        public async Task<MessageWorkflowContext?> ExecuteAsync(MessageWorkflowContext context)
        {
            var logPrefix = $"Message {context.DiscordContext.Message?.Id}";
            try
            {
                var c = await Condition.ExecuteAsync(context);
                if (c != null)
                {
                    _telemetry.Message($"[{logPrefix}] Executing workflow {OnSuccess.GetType().Name}...");
                    await _exec.ExecuteAsync(OnSuccess, c.DiscordContext);
                }
                else
                {
                    _telemetry.Message($"[{logPrefix}] Executing workflow {OnFailure.GetType().Name}...");
                    await _exec.ExecuteAsync(OnFailure, context.DiscordContext);
                }
                return c;
            }
            catch (Exception ex)
            {
                _telemetry.Error(ex.Message);
                return null;
            }
        }
    }
}
