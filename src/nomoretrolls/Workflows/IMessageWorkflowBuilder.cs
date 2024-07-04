namespace nomoretrolls.Workflows
{
    internal interface IMessageWorkflowBuilder
    {
        IMessageWorkflowBuilder Receiver(IMessageContextReceiver receiver);
        IMessageWorkflowBuilder Part(IMessageWorkflowPart part);

        IMessageWorkflowBuilder IfShoutingWorkflowEnabled();
        IMessageWorkflowBuilder IfAltCapsWorkflowEnabled();
        IMessageWorkflowBuilder IfKnockingWorkflowEnabled();
        IMessageWorkflowBuilder IfBlacklistWorkflowEnabled();
        IMessageWorkflowBuilder IfEmoteAnnotationWorkflowEnabled();
        IMessageWorkflowBuilder IfNotDmChannel();

        IMessageWorkflowBuilder If(Func<IMessageWorkflowBuilder, IMessageWorkflowBuilder> part, Func<IMessageWorkflowBuilder, IMessageWorkflowBuilder> onSuccess, Func<IMessageWorkflowBuilder, IMessageWorkflowBuilder> onFailure);
        IMessageWorkflowBuilder UserWarningsFilter(string statName, PeriodRange period);
        IMessageWorkflowBuilder BumpUserWarnings(string statName);
        IMessageWorkflowBuilder MessageIsShouting();
        IMessageWorkflowBuilder MessageIsAltCaps();
        IMessageWorkflowBuilder UserIsBlacklisted();
        IMessageWorkflowBuilder UserIsEmoteAnnotated();
        IMessageWorkflowBuilder UserHasReplies();
        IMessageWorkflowBuilder CallingEveryone();
        IMessageWorkflowBuilder SendReactionEmote();
        IMessageWorkflowBuilder SendUserReplyMessage();
        IMessageWorkflowBuilder SendDirectUserMessage();
        IMessageWorkflowBuilder DeleteUserMessage();
        IMessageWorkflowBuilder ApplyBlacklistReply();
        IMessageWorkflowBuilder ApplyShoutingReply();
        IMessageWorkflowBuilder ApplyAltCapsReply();
        IMessageWorkflowBuilder ApplyCallingEveryoneReply();
        IMessageWorkflowBuilder ApplyReactionEmote(string emotesName);
        IMessageWorkflowBuilder ApplyDirectMessage(string message);
        IMessageWorkflowBuilder BumpUserMessage();
        IMessageWorkflowBuilder Noop();

        IMessageWorkflow Build(string name);
    }
}
