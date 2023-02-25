using nomoretrolls.Emotes;

namespace nomoretrolls.Workflows.Parts
{
    internal class UserEmoteAnnotationFilter : IMessageWorkflowPart
    {
        private readonly IEmoteConfigProvider _emoteConfig;

        public UserEmoteAnnotationFilter(IEmoteConfigProvider emoteConfig)
        {
            _emoteConfig = emoteConfig;
        }

        public async Task<MessageWorkflowContext?> ExecuteAsync(MessageWorkflowContext context)
        {
            var (isMatch, emotes) = await IsCharacterMatch(context);
            if (isMatch)
            {
                return context.EmoteListName(emotes);
            }

            return null;
        }

        private async Task<(bool, string)> IsCharacterMatch(MessageWorkflowContext context)
        {
            var userId = context.AuthorId();

            var result = await _emoteConfig.GetUserEmoteAnnotationEntryAsync(userId);

            if (result == null) return (false, null);

            return (true, result.EmoteListName);
        }
    }
}
