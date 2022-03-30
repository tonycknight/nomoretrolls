namespace nomoretrolls.Workflows
{
    internal static class MessageWorkflowContextExtensions
    {
        private const string EmoteCodeKey = "EmoteCode";
        private const string ReplyTextKey = "ReplyText";

        public static MessageWorkflowContext DeepClone(this MessageWorkflowContext value)
        {
            return value with { AppData = new Dictionary<string, string>(value.AppData) };            
        }
                
        public static MessageWorkflowContext EmoteCode(this MessageWorkflowContext context, string emote)
        {
            context.AppData[EmoteCodeKey] = emote;
            return context;
        }

        public static string? EmoteCode(this MessageWorkflowContext context) => context.AppData.TryGet(EmoteCodeKey);

        public static MessageWorkflowContext ReplyText(this MessageWorkflowContext context, string value)
        {
            context.AppData[ReplyTextKey] = value;
            return context;
        }

        public static string? UserMention(this MessageWorkflowContext context) => context.DiscordContext.Message.Author.Mention;

        public static string? ReplyText(this MessageWorkflowContext contextt) => contextt.AppData.TryGet(ReplyTextKey);

        public static string Content(this MessageWorkflowContext context) => context.DiscordContext.Message.Content ?? "";

        public static ulong AuthorId(this MessageWorkflowContext context) => context.DiscordContext.Message.Author.Id;
    }
}
