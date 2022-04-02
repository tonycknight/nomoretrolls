using nomoretrolls.Workflows;
using nomoretrolls.Workflows.Parts;

namespace nomoretrolls.Commands
{
    internal class MessageWorkflowProvider
    {
        private const string shoutingStatsName = "all_shouting_messages";
        private const string shoutingStatsNotificationName = shoutingStatsName + "_notifications";
        private const string blacklistStatsName = "blacklisted_user";

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
                        b2 => b2.ApplyReactionEmote()
                                .SendReactionEmote()
                                .UserWarningsFilter(blacklistStatsName, PeriodRange.AtLeast(2, duration))
                                .ApplyBlacklistReply()
                                .SendUserReplyMessage())
                    .Build("Blacklisted user");
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
                                    b3 => b3.ApplyReactionEmote()
                                            .SendReactionEmote()
                                            .ApplyShoutingReply()
                                            .SendUserReplyMessage(),
                                    b3 => b3.UserWarningsFilter(shoutingStatsName, PeriodRange.AtLeast(3, duration))
                                            .ApplyReactionEmote()
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

    }
}
