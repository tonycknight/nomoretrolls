using System.Diagnostics.CodeAnalysis;
using MongoDB.Driver;
using nomoretrolls.Telemetry;
using Tk.Extensions.Guards;

namespace nomoretrolls.Io
{
    internal static class MongoDbExtensions
    {
        [ExcludeFromCodeCoverage]
        public static void RecreateIndex<T>(this IMongoCollection<T> col, string name, Action<string, IMongoCollection<T>> create)
        {
            try
            {
                create(name, col);
            }
            catch (MongoCommandException ex) when (ex.CodeName == "IndexOptionsConflict" || ex.CodeName == "IndexKeySpecsConflict")
            {
                col.Indexes.DropOne(name);
                create(name, col);
            }
        }

        [ExcludeFromCodeCoverage]
        public static IMongoDatabase GetDb(this Config.MongoDbConfiguration config, ITelemetry telemetry)
        {
            var settings = MongoClientSettings.FromConnectionString(config.Connection);

            settings.AllowInsecureTls = false;
            settings.UseTls = true;
            settings.ConnectTimeout = TimeSpan.FromSeconds(15);
            settings.ServerSelectionTimeout = settings.ConnectTimeout;
            settings.ClusterConfigurator = SetCommandLogging(telemetry);

            var client = new MongoClient(settings);

            return client.GetDatabase(config.DatabaseName);
        }

        public static Config.AppConfiguration GetValidateConfig(this Config.IConfigurationProvider configProvider)
        {
            var config = configProvider.GetAppConfiguration();

            config.MongoDb?.Connection.InvalidOpArg(string.IsNullOrWhiteSpace, "No connection string set.");
            config.MongoDb?.DatabaseName.InvalidOpArg(string.IsNullOrWhiteSpace, "No database name set.");
            config.MongoDb?.UserStatsCollectionName.InvalidOpArg(string.IsNullOrWhiteSpace, "Missing user stats collection name.");
            config.MongoDb?.UserBlacklistCollectionName.InvalidOpArg(string.IsNullOrWhiteSpace, "Missing user blacklist collection name.");
            config.MongoDb?.UserKnockingScheduleCollectionName.InvalidOpArg(string.IsNullOrWhiteSpace, "Missing user knocking schedule collection name.");

            return config;
        }

        private static Action<MongoDB.Driver.Core.Configuration.ClusterBuilder> SetCommandLogging(ITelemetry telemetry)
        {
            return cb =>
            {
                cb.Subscribe<MongoDB.Driver.Core.Events.CommandStartedEvent>(e =>
                {
                    telemetry.Event(new TelemetryTraceEvent()
                    {
                        Message = $"[MONGODB] {e.Command.ToString()}"
                    });
                });
            };
        }
    }
}
