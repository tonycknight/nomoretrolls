using System.Diagnostics.CodeAnalysis;
using Tk.Extensions.Collections;

namespace nomoretrolls.Config
{
    [ExcludeFromCodeCoverage]
    internal class EnvVarConfigurationProvider : IConfigurationProvider
    {
        private const string EnvVarNamePrefix = "nomoretrolls";

        public AppConfiguration GetAppConfiguration()
        {
            var result = CreateConfiguration();

            var evs = System.Environment.GetEnvironmentVariables();
            if (evs != null)
            {
                var xs = evs.OfType<System.Collections.DictionaryEntry>()
                    .Select(ev => ((string)ev.Key, (string)ev.Value))
                    .Where(t => t.Item1.StartsWith(EnvVarNamePrefix))
                    .ToDictionary(t => t.Item1, t => t.Item2);

                result.Discord.DiscordClientId = xs.GetOrDefault($"{EnvVarNamePrefix}_Discord_DiscordClientId");
                result.Discord.DiscordClientToken = xs.GetOrDefault($"{EnvVarNamePrefix}_Discord_DiscordClientToken");
                result.MongoDb.Connection = xs.GetOrDefault($"{EnvVarNamePrefix}_MongoDb_Connection");
                result.MongoDb.DatabaseName = xs.GetOrDefault($"{EnvVarNamePrefix}_MongoDb_DatabaseName");
            }
            return result;
        }

        public IConfigurationProvider SetFilePath(string filePath)
        {
            throw new NotSupportedException();
        }

        private static AppConfiguration CreateConfiguration() 
            => new AppConfiguration()
                {
                    Discord = new DiscordConfiguration()
                    {

                    },
                    MongoDb = new MongoDbConfiguration()
                    {

                    },
                    Telemetry = new TelemetryConfiguration()
                    {
                        LogMessageContent = false,
                    }
                };
    }
}
