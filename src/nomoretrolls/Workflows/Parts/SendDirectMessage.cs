using nomoretrolls.Telemetry;

namespace nomoretrolls.Workflows.Parts
{
    internal class SendDirectMessage : IMessageWorkflowPart
    {
        private readonly ITelemetry _telemetry;

        public SendDirectMessage(ITelemetry telemetry)
        {
            _telemetry = telemetry;
        }

        public async Task<MessageWorkflowContext?> ExecuteAsync(MessageWorkflowContext context)
        {
            var replyText = context.ReplyText();

            if (!string.IsNullOrWhiteSpace(replyText))
            {
                _telemetry.Event(new TelemetryEvent()
                {
                    Message = $"Sending direct message to {context.AuthorId()}..."
                });

                var replyChannel = await context.DiscordContext.Message.Author.CreateDMChannelAsync();
                await replyChannel.SendMessageAsync(replyText);
            }

            return context;
        }
    }
}
