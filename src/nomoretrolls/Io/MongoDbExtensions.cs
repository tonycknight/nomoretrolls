using System.Diagnostics.CodeAnalysis;
using MongoDB.Driver;
using Tk.Extensions;

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
        public static IMongoDatabase GetDb(this Config.MongoDbConfiguration config)
        {
            var settings = MongoClientSettings.FromConnectionString(config.Connection);
            settings.AllowInsecureTls = false;
            settings.UseTls = true;
            settings.ConnectTimeout = TimeSpan.FromSeconds(15);
            settings.ServerSelectionTimeout = settings.ConnectTimeout;

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

            return config;
        }
    }
}
