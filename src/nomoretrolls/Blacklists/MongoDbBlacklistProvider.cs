using System.Diagnostics.CodeAnalysis;
using MongoDB.Driver;
using nomoretrolls.Config;
using nomoretrolls.Io;

namespace nomoretrolls.Blacklists
{
    [ExcludeFromCodeCoverage]
    internal class MongoDbBlacklistProvider : IBlacklistProvider
    {
        private readonly Lazy<IMongoCollection<UserBlacklistEntryDto>> _blacklistCol;

        public MongoDbBlacklistProvider(Config.IConfigurationProvider configProvider)
        {
            _blacklistCol = new Lazy<IMongoCollection<UserBlacklistEntryDto>>(() => InitialiseDb(configProvider));
        }

        public async Task DeleteUserEntryAsync(ulong userId)
        {
            var filter = CreateEqualityFilter(userId);

            var col = _blacklistCol.Value;

            await col.DeleteOneAsync(filter);
        }

        public async Task<UserBlacklistEntry?> GetUserEntryAsync(ulong userId)
        {
            var filter = CreateEqualityFilter(userId);

            var col = _blacklistCol.Value;

            var result = (await col.FindAsync(filter)).FirstOrDefault();

            return result?.FromDto();
        }

        public async Task SetUserEntryAsync(UserBlacklistEntry entry)
        {
            var dto = entry.ToDto();

            var filter = CreateEqualityFilter(entry.UserId);

            var update = Builders<UserBlacklistEntryDto>.Update
                .Set(us => us.UserId, dto.UserId)
                .Set(us => us.Start, dto.Start)
                .Set(us => us.Expiry, dto.Expiry);
                
            var col = _blacklistCol.Value;

            var result = await col.UpdateOneAsync(filter, update, new UpdateOptions() { IsUpsert = true });
        }


        public async Task<IList<UserBlacklistEntry>> GetUserEntriesAsync()
        {
            var col = _blacklistCol.Value;

            var filter = Builders<UserBlacklistEntryDto>.Filter.Gt(us => us.UserId, 0);

            var result = await col.FindAsync(filter);

            return result.ToList().Select(r => r.FromDto()).ToList();
        }

        private IMongoCollection<UserBlacklistEntryDto> InitialiseDb(IConfigurationProvider configProvider)
        {
            var config = configProvider.GetValidateConfig();

            var db = config.MongoDb.GetDb();

            return CreateBlacklistCollection(config.MongoDb, db);
        }

        private IMongoCollection<UserBlacklistEntryDto> CreateBlacklistCollection(MongoDbConfiguration config, IMongoDatabase db)
        {
            var col = db.GetCollection<UserBlacklistEntryDto>(config.UserBlacklistCollectionName);
            if (col == null)
            {
                var opts = new CreateCollectionOptions();
                db.CreateCollection(config.UserBlacklistCollectionName, opts);
            }
            col = db.GetCollection<UserBlacklistEntryDto>(config.UserBlacklistCollectionName);

            col.RecreateIndex("unique", (n, c) => CreateUniqueIndex(n, col));
            col.RecreateIndex("ttl", (n, c) => CreateTtlIndex(n, col));

            return col;
        }

        private void CreateUniqueIndex(string name, IMongoCollection<UserBlacklistEntryDto> col)
        {
            var build = Builders<UserBlacklistEntryDto>.IndexKeys;

            var uniqueIndexModel = new CreateIndexModel<UserBlacklistEntryDto>(
                    build.Ascending(x => x.UserId),
                    new CreateIndexOptions() { Name = name, Unique = true, Background = false });

            col.Indexes.CreateOne(uniqueIndexModel);
        }

        private void CreateTtlIndex(string name, IMongoCollection<UserBlacklistEntryDto> col)
        {
            var expiresAfter = TimeSpan.FromSeconds(1);

            var build = Builders<UserBlacklistEntryDto>.IndexKeys;

            var ttlIndexModel = new CreateIndexModel<UserBlacklistEntryDto>(
                    build.Ascending(x => x.Expiry),
                    new CreateIndexOptions() { Name = name, Unique = false, ExpireAfter = expiresAfter, Background = false });

            col.Indexes.CreateOne(ttlIndexModel);
        }

        private FilterDefinition<UserBlacklistEntryDto> CreateEqualityFilter(ulong userId)
            => Builders<UserBlacklistEntryDto>.Filter.Eq(us => us.UserId, userId);

    }
}
