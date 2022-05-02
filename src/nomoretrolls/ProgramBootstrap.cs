using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Tk.Extensions;
using Tk.Extensions.Reflection;

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



        public static string GetDescription() => 
            GetVersionDescription().Concat(GetCopyrightDescriptions())
                .Where(x => x != null)
                .Join(Environment.NewLine);

        public static IEnumerable<string> GetVersionDescription()
        {
            var attrs = typeof(ProgramBootstrap).Assembly.GetCustomAttributes();

            return new[]
                {
                    attrs.GetAttributeValue<AssemblyProductAttribute>(a => a.Product),
                    attrs.GetAttributeValue<AssemblyDescriptionAttribute>(a => a.Description),
                    "",
                    $"{attrs.GetAttributeValue<AssemblyInformationalVersionAttribute>(a => a.InformationalVersion).Format("Version {0} beta")}",
            };
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
