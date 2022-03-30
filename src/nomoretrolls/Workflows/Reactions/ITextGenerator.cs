namespace nomoretrolls.Workflows.Reactions
{
    internal interface ITextGenerator
    {
        string GenerateReply(string userMention);
    }
}
