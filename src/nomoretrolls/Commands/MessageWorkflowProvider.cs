using nomoretrolls.Workflows;
using nomoretrolls.Workflows.Parts;

namespace nomoretrolls.Commands
{
    internal class MessageWorkflowProvider
    {
        private const string shoutingStatsName = "all_shouting_messages";
        private const string shoutingStatsNotificationName = shoutingStatsName + "_notifications";
        private const string blacklistStatsName = "blacklisted_user";
        private const string blacklistStatsNotificationName = blacklistStatsName + "_notifications";

        private readonly IMessageWorkflowFactory _wfFactory;

        public MessageWorkflowProvider(IMessageWorkflowFactory wfFactory)
        {
            _wfFactory = wfFactory;
        }

        public IMessageWorkflow CreateBlacklistedUserWorkflow()
        {        
            var duration = TimeSpan.FromMinutes(1);

            return _wfFactory.CreateBuilder()
                    .Receiver(new MessageReceiver())
                    .IfBlacklistWorkflowEnabled()
                    .UserIsBlacklisted()
                    .BumpUserWarnings(blacklistStatsName)
                    .If(b2 => b2.UserWarningsFilter(blacklistStatsName, PeriodRange.AtLeast(5, duration)),
                        b2 => b2.DeleteUserMessage(),
                        b2 => b2.ApplyReactionEmote("blacklist")
                                .SendReactionEmote()
                                .UserWarningsFilter(blacklistStatsName, PeriodRange.AtLeast(2, duration))
                                .ApplyBlacklistReply()
                                .SendUserReplyMessage())
                    .Build("Blacklisted user");
        }


        public IMessageWorkflow CreateBlacklistedPersonalReplyWorkflow()
        {
            var window = TimeSpan.FromDays(1);

            return _wfFactory.CreateBuilder()
                    .Receiver(new MessageReceiver())
                    .IfBlacklistWorkflowEnabled()
                    .UserIsBlacklisted()
                    .UserWarningsFilter(blacklistStatsName, PeriodRange.AtLeast(1, window))
                    .UserWarningsFilter(blacklistStatsNotificationName, PeriodRange.AtMost(0, window))
                    .BumpUserWarnings(blacklistStatsNotificationName)
                    .ApplyDirectMessage("{0} You've been blacklisted. Please behave.")
                    .SendDirectUserMessage()
                    .Build("Blacklisted user DM");
        }

        public IMessageWorkflow CreateShoutingWorkflow()
        {
            var duration = TimeSpan.FromMinutes(5);

            return _wfFactory.CreateBuilder()
                    .Receiver(new MessageReceiver())
                    .IfShoutingWorkflowEnabled()
                    .MessageIsShouting()
                    .BumpUserWarnings(shoutingStatsName)
                    .If(b2 => b2.UserWarningsFilter(shoutingStatsName, PeriodRange.AtLeast(8, duration)),
                        b2 => b2.DeleteUserMessage(),
                        b2 => b2.If(b3 => b3.UserWarningsFilter(shoutingStatsName, PeriodRange.AtLeast(5, duration)),
                                    b3 => b3.ApplyReactionEmote("shouting")
                                            .SendReactionEmote()
                                            .ApplyShoutingReply()
                                            .SendUserReplyMessage(),
                                    b3 => b3.UserWarningsFilter(shoutingStatsName, PeriodRange.AtLeast(3, duration))
                                            .ApplyReactionEmote("shouting")
                                            .SendReactionEmote()))
                    .Build("Shouting user");
        }

        public IMessageWorkflow CreateShoutingPersonalReplyWorkflow()
        {            
            var window = TimeSpan.FromDays(1);

            return _wfFactory.CreateBuilder()
                    .Receiver(new MessageReceiver())
                    .IfShoutingWorkflowEnabled()
                    .MessageIsShouting()
                    .UserWarningsFilter(shoutingStatsName, PeriodRange.AtLeast(1, window))
                    .UserWarningsFilter(shoutingStatsNotificationName, PeriodRange.AtMost(0, window))
                    .BumpUserWarnings(shoutingStatsNotificationName)
                    .ApplyDirectMessage("{0} You have been warned. No more shouting.")
                    .SendDirectUserMessage()
                    .Build("Shouting user DM");
        }

        public IMessageWorkflow CreateAutoEmoteWorkflow() 
            => _wfFactory.CreateBuilder()
                    .Receiver(new MessageReceiver())
                    .IfEmoteAnnotationWorkflowEnabled()
                    .UserIsEmoteAnnotated()
                    .ApplyReactionEmote(null)
                    .SendReactionEmote()
                    .Build("Emote Annotation");

    }
}
