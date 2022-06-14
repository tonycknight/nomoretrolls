using nomoretrolls.Emotes;

namespace nomoretrolls.Workflows.Parts
{
    internal class ApplyReactionEmote : IMessageWorkflowPart
    {
        private readonly IEmoteGenerator _emoteGenerator;

        public ApplyReactionEmote(IEmoteGenerator emoteGenerator)
        {
            _emoteGenerator = emoteGenerator;
        }

        public Task<MessageWorkflowContext?> ExecuteAsync(MessageWorkflowContext context)
        {
            var result = context.DeepClone()
                        .EmoteCode(_emoteGenerator.PickDisapproveEmotes());

            return Task.FromResult(result);
        }
    }
}
