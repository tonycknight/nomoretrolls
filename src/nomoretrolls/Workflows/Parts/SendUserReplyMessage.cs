using nomoretrolls.Telemetry;

namespace nomoretrolls.Workflows.Parts
{
    internal class SendUserReplyMessage : IMessageWorkflowPart
    {
        private readonly ITelemetry _telemetry;

        public SendUserReplyMessage(ITelemetry telemetry) 
        {
            _telemetry = telemetry;
        }

        public async Task<MessageWorkflowContext?> ExecuteAsync(MessageWorkflowContext context)
        {
            var replyText = context.ReplyText();
            
            if (!string.IsNullOrWhiteSpace(replyText))
            {
                var msg = context.DiscordContext.Message;
                var msgRef = new Discord.MessageReference(msg.Id);
                                
                _telemetry.Event(new TelemetryEvent()
                {
                    Message = $"Sending reply to {context.AuthorId()}..."
                });

                await msg.Channel.SendMessageAsync(text: replyText, messageReference: msgRef);                                
             }

            return context;
        }
    }
}
