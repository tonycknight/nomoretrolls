using System.Diagnostics.CodeAnalysis;
using MongoDB.Driver;
using nomoretrolls.Config;
using nomoretrolls.Emotes;
using nomoretrolls.Io;
using nomoretrolls.Telemetry;

namespace nomoretrolls.Replies
{
    [ExcludeFromCodeCoverage]
    internal class MongoDbReplyProvider : IReplyProvider
    {
        private readonly Lazy<IMongoCollection<UserReplyEntryDto>> _col;
        private readonly ITelemetry _telemetry;

        public MongoDbReplyProvider(Config.IConfigurationProvider configProvider, ITelemetry telemetry)
        {
            _col = new Lazy<IMongoCollection<UserReplyEntryDto>>(() => InitialiseDb(configProvider));
            _telemetry = telemetry;
        }

        public async Task DeleteUserEntryAsync(ulong userId)
        {
            var filter = CreateEqualityFilter(userId);

            await _col.Value.DeleteOneAsync(filter);
        }

        public async Task<IList<UserReplyEntry>> GetUserEntriesAsync()
        {
            var filter = Builders<UserReplyEntryDto>.Filter.Gt(us => us.UserId, 0);
                        
            var result = await _col.Value.FindAsync(filter);

            return result.ToList().Select(r => r.FromDto()).ToList();
        }

        public async Task<UserReplyEntry> GetUserEntriesAsync(ulong userId)
        {
            var filter = CreateEqualityFilter(userId);
            var result = (await _col.Value.FindAsync(filter)).FirstOrDefault();

            return result?.FromDto();
        }

        public async Task SetUserEntryAsync(UserReplyEntry entry)
        {
            var dto = entry.ToDto();

            var filter = CreateEqualityFilter(entry.UserId);

            var update = Builders<UserReplyEntryDto>.Update
                .Set(us => us.UserId, dto.UserId)
                .Set(us => us.Start, dto.Start)
                .Set(us => us.Message, dto.Message)
                .Set(us => us.Expiry, dto.Expiry);

            var result = await _col.Value.UpdateOneAsync(filter, update, new UpdateOptions() { IsUpsert = true });

        }

        private IMongoCollection<UserReplyEntryDto> InitialiseDb(IConfigurationProvider configProvider)
        {
            var config = configProvider.GetValidateConfig();

            var db = config.MongoDb.GetDb(_telemetry);

            return CreateCollection(config.MongoDb, db);
        }

        private IMongoCollection<UserReplyEntryDto> CreateCollection(MongoDbConfiguration config, IMongoDatabase db)
        {
            var col = db.GetCollection<UserReplyEntryDto>(config.UserReplyCollectionName);
            if (col == null)
            {
                var opts = new CreateCollectionOptions();
                db.CreateCollection(config.UserReplyCollectionName, opts);
            }
            col = db.GetCollection<UserReplyEntryDto>(config.UserReplyCollectionName);

            col.RecreateIndex("unique", (n, c) => CreateUniqueIndex(n, col));
            col.RecreateIndex("ttl", (n, c) => CreateTtlIndex(n, col));

            return col;
        }

        private void CreateUniqueIndex(string name, IMongoCollection<UserReplyEntryDto> col)
        {
            var build = Builders<UserReplyEntryDto>.IndexKeys;

            var uniqueIndexModel = new CreateIndexModel<UserReplyEntryDto>(
                    build.Ascending(x => x.UserId),
                    new CreateIndexOptions() { Name = name, Unique = true, Background = false });

            col.Indexes.CreateOne(uniqueIndexModel);
        }

        private void CreateTtlIndex(string name, IMongoCollection<UserReplyEntryDto> col)
        {
            var expiresAfter = TimeSpan.FromSeconds(1);

            var build = Builders<UserReplyEntryDto>.IndexKeys;

            var ttlIndexModel = new CreateIndexModel<UserReplyEntryDto>(
                    build.Ascending(x => x.Expiry),
                    new CreateIndexOptions() { Name = name, Unique = false, ExpireAfter = expiresAfter, Background = false });

            col.Indexes.CreateOne(ttlIndexModel);
        }

        private FilterDefinition<UserReplyEntryDto> CreateEqualityFilter(ulong userId)
            => Builders<UserReplyEntryDto>.Filter.Eq(us => us.UserId, userId);
    }
}
