namespace nomoretrolls.Workflows
{
    internal interface IMessageWorkflowBuilder
    {
        IMessageWorkflowBuilder Receiver(IMessageContextReceiver receiver);
        IMessageWorkflowBuilder Part(IMessageWorkflowPart part);

        IMessageWorkflowBuilder IfShoutingFilterEnabled();
        IMessageWorkflowBuilder IfBlacklistFilterEnabled();

        IMessageWorkflowBuilder If(Func<IMessageWorkflowBuilder, IMessageWorkflowBuilder> part, Func<IMessageWorkflowBuilder, IMessageWorkflowBuilder> onSuccess, Func<IMessageWorkflowBuilder, IMessageWorkflowBuilder> onFailure);
        IMessageWorkflowBuilder UserWarningsFilter(string statName, PeriodRange period);
        IMessageWorkflowBuilder BumpUserWarnings(string statName);
        IMessageWorkflowBuilder MessageIsShoutingFilter();
        IMessageWorkflowBuilder UserBlacklistFilter();
        IMessageWorkflowBuilder SendReactionEmote();
        IMessageWorkflowBuilder SendUserReplyMessage();
        IMessageWorkflowBuilder SendDirectUserMessage();
        IMessageWorkflowBuilder DeleteUserMessage();
        IMessageWorkflowBuilder ApplyBlacklistReply();
        IMessageWorkflowBuilder ApplyShoutingReply();
        IMessageWorkflowBuilder ApplyReactionEmote();
        IMessageWorkflowBuilder ApplyDirectMessage(string message);
        IMessageWorkflowBuilder Noop();

        IMessageWorkflow Build();
    }
}
