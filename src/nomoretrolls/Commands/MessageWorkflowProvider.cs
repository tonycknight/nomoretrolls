using nomoretrolls.Workflows;
using nomoretrolls.Workflows.Parts;

namespace nomoretrolls.Commands
{
    internal class MessageWorkflowProvider
    {
        private const string capitalsStatsName = "all_capitals_messages";
        private const string capitalStatsNotificationName = capitalsStatsName + "_notifications";
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
                    .IfBlacklistFilterEnabled()
                    .UserBlacklistFilter()
                    .BumpUserWarnings(blacklistStatsName)
                    .If(b2 => b2.UserWarningsFilter(blacklistStatsName, PeriodRange.AtLeast(5, duration)),
                        b2 => b2.DeleteUserMessage(),
                        b2 => b2.ApplyReactionEmote()
                                .SendReactionEmote()
                                .UserWarningsFilter(blacklistStatsName, PeriodRange.AtLeast(2, duration))
                                .ApplyBlacklistReply()
                                .SendUserReplyMessage())
                    .Build();
        }

        public IMessageWorkflow CreateNoCapitalsWorkflow()
        {
            var duration = TimeSpan.FromMinutes(5);

            return _wfFactory.CreateBuilder()
                    .Receiver(new MessageReceiver())
                    .IfCapitalsFilterEnabled()
                    .MessageIsCapitalsFilter()
                    .BumpUserWarnings(capitalsStatsName)
                    .If(b2 => b2.UserWarningsFilter(capitalsStatsName, PeriodRange.AtLeast(8, duration)),
                        b2 => b2.DeleteUserMessage(),
                        b2 => b2.If(b3 => b3.UserWarningsFilter(capitalsStatsName, PeriodRange.AtLeast(5, duration)),
                                    b3 => b3.ApplyShoutingReply()
                                            .SendUserReplyMessage(),
                                    b3 => b3.UserWarningsFilter(capitalsStatsName, PeriodRange.AtLeast(3, duration))
                                            .ApplyReactionEmote()
                                            .SendReactionEmote()))
                    .Build();
        }

        public IMessageWorkflow CreateNoCapitalsPersonalReplyWorkflow()
        {            
            var window = TimeSpan.FromDays(1);

            return _wfFactory.CreateBuilder()
                    .Receiver(new MessageReceiver())
                    .IfCapitalsFilterEnabled()
                    .MessageIsCapitalsFilter()
                    .UserWarningsFilter(capitalsStatsName, PeriodRange.AtLeast(1, window))
                    .UserWarningsFilter(capitalStatsNotificationName, PeriodRange.AtMost(0, window))
                    .BumpUserWarnings(capitalStatsNotificationName)
                    .ApplyDirectMessage("{0} You have been warned. No more shouting.")
                    .SendDirectUserMessage()
                    .Build();
        }

    }
}
