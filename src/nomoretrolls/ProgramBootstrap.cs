using Microsoft.Extensions.DependencyInjection;

namespace nomoretrolls
{
    internal class ProgramBootstrap
    {
        public static IServiceProvider CreateServiceCollection() => 
            new ServiceCollection()
                .AddSingleton<Config.IConfigurationProvider, Config.FileConfigurationProvider>()
                .AddSingleton<IList<Telemetry.ITelemetry>>(sp => new Telemetry.ITelemetry[] { new Telemetry.ConsoleTelemetry() })
                .AddSingleton<Telemetry.ITelemetry, Telemetry.AggregatedTelemetry>()
                .AddSingleton<Io.IIoProvider, Io.IoProvider>()
                .AddSingleton<Workflows.Reactions.IBlacklistReplyTextGenerator, Workflows.Reactions.BlacklistReplyTextGenerator>()
                .AddSingleton<Workflows.Reactions.IEmoteGenerator, Workflows.Reactions.EmoteGenerator>()
                .AddSingleton<Workflows.Reactions.IShoutingReplyTextGenerator, Workflows.Reactions.ShoutingReplyTextGenerator>()
                .AddSingleton<Workflows.IMessageWorkflowFactory, Workflows.MessageWorkflowFactory>()
                .AddTransient<Workflows.IMessageWorkflowExecutor, Workflows.MessageWorkflowExecutor>()
                .AddSingleton<Statistics.IUserStatisticsProvider, Statistics.MongoDbUserStatisticsProvider>()
                .AddSingleton<Blacklists.IBlacklistProvider, Blacklists.MongoDbBlacklistProvider>()
                .AddSingleton<Config.MemoryWorkflowConfigurationRepository>()
                .AddSingleton < Config.MongoDbWorkflowConfigurationRepository>()
                .AddSingleton < Config.IWorkflowConfigurationRepository>(sp => 
                    new Config.WorkflowConfigurationRepository(sp.GetService<Config.MemoryWorkflowConfigurationRepository>(), sp.GetService<Config.MongoDbWorkflowConfigurationRepository>()))
                .BuildServiceProvider();
    }
}
