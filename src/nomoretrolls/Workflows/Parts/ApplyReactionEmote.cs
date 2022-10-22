using nomoretrolls.Emotes;

namespace nomoretrolls.Workflows.Parts
{
    internal class ApplyReactionEmote : IMessageWorkflowPart
    {
        private readonly IEmoteGenerator _emoteGenerator;
        private readonly string _emotesName;

        public ApplyReactionEmote(IEmoteGenerator emoteGenerator, string emotesName)
        {
            _emoteGenerator = emoteGenerator;
            _emotesName = emotesName;
        }

        public async Task<MessageWorkflowContext?> ExecuteAsync(MessageWorkflowContext context)
        {
            var emotesName = _emotesName ?? context.EmoteListName();

            var e = await _emoteGenerator.PickEmoteAsync(emotesName);

            return context.DeepClone().EmoteCode(e);
        }
    }
}
