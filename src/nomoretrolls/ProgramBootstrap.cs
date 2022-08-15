using System.Reflection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using nomoretrolls.Emotes;
using Tk.Extensions;
using Tk.Extensions.Reflection;
using Tk.Extensions.Time;

namespace nomoretrolls
{
    internal class ProgramBootstrap
    {
        public static IServiceProvider CreateServiceCollection() =>
            new ServiceCollection()
                .AddSingleton<Config.FileConfigurationProvider>()
                .AddSingleton<Config.EnvVarConfigurationProvider>()
                .AddSingleton<Config.IConfigurationProvider, Config.ConfigurationProvider>()
                .AddSingleton<IList<Telemetry.ITelemetry>>(sp => new Telemetry.ITelemetry[] { new Telemetry.ConsoleTelemetry() })
                .AddSingleton<Telemetry.ITelemetry, Telemetry.AggregatedTelemetry>()
                .AddSingleton<Io.IIoProvider, Io.IoProvider>()
                .AddMemoryCache()
                .AddSingleton<Tk.Extensions.Time.ITimeProvider, Tk.Extensions.Time.TimeProvider>()
                .AddSingleton<Messaging.IDiscordMessagingClientProvider, Messaging.DiscordMessagingClientProvider>()
                .AddSingleton<Workflows.Reactions.IBlacklistReplyTextGenerator, Workflows.Reactions.BlacklistReplyTextGenerator>()
                .AddSingleton<IEmoteRepository, EmoteRepository>()
                .AddSingleton<IEmoteGenerator, EmoteGenerator>()
                .AddSingleton<Workflows.Reactions.IShoutingReplyTextGenerator, Workflows.Reactions.ShoutingReplyTextGenerator>()
                .AddSingleton<Workflows.IMessageWorkflowFactory, Workflows.MessageWorkflowFactory>()
                .AddTransient<Workflows.IMessageWorkflowExecutor, Workflows.MessageWorkflowExecutor>()
                .AddSingleton<Statistics.IUserStatisticsProvider, Statistics.MongoDbUserStatisticsProvider>()

                .AddSingleton<Blacklists.MongoDbBlacklistProvider>()
                .AddSingleton<Blacklists.IBlacklistProvider>((IServiceProvider sp) => new Blacklists.CachedBlacklistProvider(sp.GetRequiredService<IMemoryCache>(),
                                                                                                                             sp.GetRequiredService<Blacklists.MongoDbBlacklistProvider>()))

                .AddSingleton<MongoDbEmoteConfigProvider>()                
                .AddSingleton<IEmoteConfigProvider>((IServiceProvider sp) => new CachedEmoteConfigProvider(sp.GetRequiredService<IMemoryCache>(), 
                                                                                                           sp.GetRequiredService<MongoDbEmoteConfigProvider>()))
                
                .AddSingleton<Config.MemoryWorkflowConfigurationRepository>()
                .AddSingleton<Config.MongoDbWorkflowConfigurationRepository>()
                .AddSingleton<Config.IWorkflowConfigurationRepository>(sp => 
                    new Config.WorkflowConfigurationRepository(sp.GetService<Config.MemoryWorkflowConfigurationRepository>(), 
                                                               sp.GetService<Config.MongoDbWorkflowConfigurationRepository>()))

                .AddSingleton<Knocking.MongoDbKnockingScheduleRepository>()
                .AddSingleton<Knocking.IKnockingScheduleRepository>((IServiceProvider sp) 
                => new Knocking.CachedKnockingScheduleRepository(sp.GetRequiredService<IMemoryCache>(),
                                                                 sp.GetRequiredService<Knocking.MongoDbKnockingScheduleRepository>(),
                                                                 sp.GetRequiredService<ITimeProvider>()))
                .AddSingleton<Scheduling.IJobScheduler, Scheduling.JobScheduler>()
                .AddSingleton<Knocking.KnockingScheduleJob>()
                .BuildServiceProvider();

        public static IEnumerable<string> GetProductDescription()
        {
            var attrs = typeof(ProgramBootstrap).Assembly.GetCustomAttributes();

            return new[]
                {
                    attrs.GetAttributeValue<AssemblyProductAttribute>(a => a.Product),
                    attrs.GetAttributeValue<AssemblyDescriptionAttribute>(a => a.Description),                    
            };
        }

        public static IEnumerable<string> GetVersionDescription()
        {
            var attrs = typeof(ProgramBootstrap).Assembly.GetCustomAttributes();

            yield return $"{attrs.GetAttributeValue<AssemblyInformationalVersionAttribute>(a => a.InformationalVersion).Format("Version {0} beta")}";
        }

        public static IEnumerable<string> GetCopyrightDescriptions()
        {
            var attrs = typeof(ProgramBootstrap).Assembly.GetCustomAttributes();

            return new[]
                {
                    attrs.GetAttributeValue<AssemblyCopyrightAttribute>(a => a.Copyright),
                    "You can find the repository at https://github.com/tonycknight/nomoretrolls",
                };
        }
    }
}
