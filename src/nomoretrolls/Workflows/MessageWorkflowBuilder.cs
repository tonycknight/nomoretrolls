using Microsoft.Extensions.DependencyInjection;
using nomoretrolls.Blacklists;
using nomoretrolls.Emotes;
using nomoretrolls.Statistics;
using nomoretrolls.Telemetry;
using nomoretrolls.Workflows.Reactions;
using Tk.Extensions.Guards;

namespace nomoretrolls.Workflows
{
    internal sealed class MessageWorkflowBuilder : IMessageWorkflowBuilder
    {

        private readonly List<IMessageWorkflowPart> _parts = new List<IMessageWorkflowPart>();
        private readonly IServiceProvider _serviceProvider;
        private IMessageContextReceiver? _receiver;

        public MessageWorkflowBuilder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider.ArgNotNull(nameof(serviceProvider));
        }

        public IMessageWorkflow Build(string name)
        {
            _receiver.InvalidOpArg(r => r == null, "Missing receiver.");
            _parts.InvalidOpArg(ps => ps.Count == 0, "Missing parts.");

            return new MessageWorkflow(_receiver, _parts, name);
        }

        public IMessageWorkflowBuilder Part(IMessageWorkflowPart part)
        {
            _parts.Add(part);
            return this;
        }

        public IMessageWorkflowBuilder Receiver(IMessageContextReceiver receiver)
        {
            _receiver = receiver;
            return this;
        }

        public IMessageWorkflowBuilder IfShoutingWorkflowEnabled()
            => this.Part(new Parts.WorkflowConfigEnabled(_serviceProvider.GetService<Config.IWorkflowConfigurationRepository>(), Config.IWorkflowConfigurationRepository.ShoutingWorkflow));

        public IMessageWorkflowBuilder IfBlacklistWorkflowEnabled()
            => this.Part(new Parts.WorkflowConfigEnabled(_serviceProvider.GetService<Config.IWorkflowConfigurationRepository>(), Config.IWorkflowConfigurationRepository.BlacklistWorkflow));

        public IMessageWorkflowBuilder IfAltCapsWorkflowEnabled()
            => this.Part(new Parts.WorkflowConfigEnabled(_serviceProvider.GetService<Config.IWorkflowConfigurationRepository>(), Config.IWorkflowConfigurationRepository.AltCapsWorkflow));

        public IMessageWorkflowBuilder IfKnockingWorkflowEnabled()
            => this.Part(new Parts.WorkflowConfigEnabled(_serviceProvider.GetService<Config.IWorkflowConfigurationRepository>(), Config.IWorkflowConfigurationRepository.KnockingWorkflow));

        public IMessageWorkflowBuilder IfEmoteAnnotationWorkflowEnabled()
            => this.Part(new Parts.WorkflowConfigEnabled(_serviceProvider.GetService<Config.IWorkflowConfigurationRepository>(), Config.IWorkflowConfigurationRepository.EmoteAnnotationWorkflow));

        public IMessageWorkflowBuilder IfCallingEveryoneWorkflowEnabled()
            => this.Part(new Parts.WorkflowConfigEnabled(_serviceProvider.GetService<Config.IWorkflowConfigurationRepository>(), Config.IWorkflowConfigurationRepository.EveryoneWorkflow));

        public IMessageWorkflowBuilder IfNotDmChannel()
            => this.Part(new Parts.NonDmChannelFilter());

        public IMessageWorkflowBuilder If(Func<IMessageWorkflowBuilder, IMessageWorkflowBuilder> condition, Func<IMessageWorkflowBuilder, IMessageWorkflowBuilder> onSuccess, Func<IMessageWorkflowBuilder, IMessageWorkflowBuilder> onFailure)
        {
            var b1 = new MessageWorkflowBuilder(_serviceProvider).Receiver(_receiver);
            var wfCondition = condition(b1).Build("Condition");

            var b2 = new MessageWorkflowBuilder(_serviceProvider).Receiver(_receiver);
            var wfSuccess = onSuccess(b2).Build("Success");

            var b3 = new MessageWorkflowBuilder(_serviceProvider).Receiver(_receiver);
            var wfFailure = onFailure(b3).Build("Failure");

            var part = new Parts.IfPart(_serviceProvider.GetService<ITelemetry>(),
                _serviceProvider.GetService<IMessageWorkflowExecutor>(),
                wfCondition.Parts.First(),
                wfSuccess,
                wfFailure);

            return this.Part(part);
        }

        public IMessageWorkflowBuilder MessageIsShouting()
            => this.Part(new Parts.MessageIsShoutingFilter(_serviceProvider.GetService<ITelemetry>()));

        public IMessageWorkflowBuilder MessageIsAltCaps()
            => this.Part(new Parts.MessageIsAltCapsFilter(_serviceProvider.GetService<ITelemetry>()));

        public IMessageWorkflowBuilder Noop()
            => this.Part(new Parts.Noop());

        public IMessageWorkflowBuilder SendReactionEmote()
            => this.Part(new Parts.SendReactionEmote(_serviceProvider.GetService<ITelemetry>()));

        public IMessageWorkflowBuilder SendUserReplyMessage()
            => this.Part(new Parts.SendUserReplyMessage(_serviceProvider.GetService<ITelemetry>()));

        public IMessageWorkflowBuilder UserWarningsFilter(string statName, PeriodRange period)
            => this.Part(new Parts.UserWarningsFilter(_serviceProvider.GetService<IUserStatisticsProvider>(), statName, period));

        public IMessageWorkflowBuilder BumpUserWarnings(string statName)
            => this.Part(new Parts.BumpUserWarnings(_serviceProvider.GetService<IUserStatisticsProvider>(), statName));

        public IMessageWorkflowBuilder UserIsBlacklisted()
            => this.Part(new Parts.UserBlacklistFilter(_serviceProvider.GetService<IBlacklistProvider>()));

        public IMessageWorkflowBuilder UserIsEmoteAnnotated()
            => this.Part(new Parts.UserEmoteAnnotationFilter(_serviceProvider.GetService<IEmoteConfigProvider>()));

        public IMessageWorkflowBuilder UserHasReplies()
            => this.Part(new Parts.UserReplyFilter(_serviceProvider.GetService<Replies.IReplyProvider>()));

        public IMessageWorkflowBuilder CallingEveryone()
            => this.Part(new Parts.CallingEveryoneFilter());

        public IMessageWorkflowBuilder SendDirectUserMessage()
            => this.Part(new Parts.SendDirectMessage(_serviceProvider.GetService<ITelemetry>()));

        public IMessageWorkflowBuilder DeleteUserMessage()
            => this.Part(new Parts.DeleteUserMessage());

        public IMessageWorkflowBuilder ApplyBlacklistReply()
            => this.Part(new Parts.ApplyBlacklistReplyText(_serviceProvider.GetService<IBlacklistReplyTextGenerator>()));

        public IMessageWorkflowBuilder ApplyShoutingReply()
            => this.Part(new Parts.ApplyShoutingReplyText(_serviceProvider.GetService<IShoutingReplyTextGenerator>()));

        public IMessageWorkflowBuilder ApplyAltCapsReply()
            => this.Part(new Parts.ApplyAltCapsReplyText(_serviceProvider.GetService<IAltCapsReplyTextGenerator>()));

        public IMessageWorkflowBuilder ApplyCallingEveryoneReply()
            => this.Part(new Parts.ApplyCallingEveryoneReplyText(new CallingEveryoneReplyTextGenerator()));

        public IMessageWorkflowBuilder ApplyReply()
            => this.Part(new Parts.SendUserReplyMessage(_serviceProvider.GetService<ITelemetry>()));

        public IMessageWorkflowBuilder ApplyReactionEmote(string emotesName)
            => this.Part(new Parts.ApplyReactionEmote(_serviceProvider.GetService<IEmoteGenerator>(), emotesName));

        public IMessageWorkflowBuilder ApplyDirectMessage(string message)
            => this.Part(new Parts.ApplyReplyText(new ArbitraryTextGenerator(message)));

        public IMessageWorkflowBuilder BumpUserMessage()
            => this.Part(new Parts.BumpUserMessage(_serviceProvider.GetService<IUserStatisticsProvider>()));
    }

}
