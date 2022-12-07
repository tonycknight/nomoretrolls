using nomoretrolls.Telemetry;

namespace nomoretrolls.Workflows.Parts
{
    internal class SendReactionEmote : IMessageWorkflowPart
    {
        private readonly ITelemetry _telemetry;

        public SendReactionEmote(ITelemetry telemetry)
        {
            _telemetry = telemetry;
        }

        public async Task<MessageWorkflowContext?> ExecuteAsync(MessageWorkflowContext context)
        {
            var emoteCode = context.EmoteCode();
            if (!string.IsNullOrWhiteSpace(emoteCode))
            {
                _telemetry.Event(new TelemetryEvent()
                {
                    Message = $"Sending user emote to {context.AuthorId()}..."
                });

                var emo = new Discord.Emoji(emoteCode);
                await context.DiscordContext.Message.AddReactionAsync(emo);
            }
             return context;
        }
    }
}
