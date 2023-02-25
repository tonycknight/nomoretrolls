using System.Diagnostics.CodeAnalysis;
using MongoDB.Driver;
using nomoretrolls.Config;
using nomoretrolls.Io;
using nomoretrolls.Telemetry;

namespace nomoretrolls.Emotes
{
    [ExcludeFromCodeCoverage]
    internal class MongoDbEmoteConfigProvider : IEmoteConfigProvider
    {
        private readonly Lazy<IMongoCollection<UserEmoteAnnotationEntryDto>> _col;
        private readonly ITelemetry _telemetry;

        public MongoDbEmoteConfigProvider(IConfigurationProvider configProvider, ITelemetry telemetry)
        {
            _col = new Lazy<IMongoCollection<UserEmoteAnnotationEntryDto>>(() => InitialiseDb(configProvider));
            _telemetry = telemetry;
        }

        public async Task DeleteUserEmoteAnnotationEntryAsync(ulong userId)
        {
            var filter = CreateEqualityFilter(userId);

            var col = _col.Value;

            await col.DeleteOneAsync(filter);
        }

        public async Task<IList<UserEmoteAnnotationEntry>> GetUserEmoteAnnotationEntriesAsync()
        {
            var col = _col.Value;

            var filter = Builders<UserEmoteAnnotationEntryDto>.Filter.Gt(us => us.UserId, 0);

            var result = await col.FindAsync(filter);

            return result.ToList().Select(r => r.FromDto()).ToList();
        }

        public async Task<UserEmoteAnnotationEntry?> GetUserEmoteAnnotationEntryAsync(ulong userId)
        {
            var filter = CreateEqualityFilter(userId);

            var col = _col.Value;

            var result = (await col.FindAsync(filter)).FirstOrDefault();

            return result?.FromDto();
        }

        public async Task SetUserEmoteAnnotationEntryAsync(UserEmoteAnnotationEntry entry)
        {
            var dto = entry.ToDto();

            var filter = CreateEqualityFilter(entry.UserId);

            var update = Builders<UserEmoteAnnotationEntryDto>.Update
                .Set(us => us.UserId, dto.UserId)
                .Set(us => us.Start, dto.Start)
                .Set(us => us.Expiry, dto.Expiry)
                .Set(us => us.EmoteListName, dto.EmoteListName);

            var col = _col.Value;

            var result = await col.UpdateOneAsync(filter, update, new UpdateOptions() { IsUpsert = true });
        }

        private IMongoCollection<UserEmoteAnnotationEntryDto> InitialiseDb(IConfigurationProvider configProvider)
        {
            var config = configProvider.GetValidateConfig();

            var db = config.MongoDb.GetDb(_telemetry);

            return CreateCollection(config.MongoDb, db);
        }

        private IMongoCollection<UserEmoteAnnotationEntryDto> CreateCollection(MongoDbConfiguration config, IMongoDatabase db)
        {
            var col = db.GetCollection<UserEmoteAnnotationEntryDto>(config.UserEmoteAnnotationsCollectionName);
            if (col == null)
            {
                var opts = new CreateCollectionOptions();
                db.CreateCollection(config.UserEmoteAnnotationsCollectionName, opts);
            }
            col = db.GetCollection<UserEmoteAnnotationEntryDto>(config.UserEmoteAnnotationsCollectionName);

            col.RecreateIndex("unique", (n, c) => CreateUniqueIndex(n, col));
            col.RecreateIndex("ttl", (n, c) => CreateTtlIndex(n, col));

            return col;
        }

        private void CreateUniqueIndex(string name, IMongoCollection<UserEmoteAnnotationEntryDto> col)
        {
            var build = Builders<UserEmoteAnnotationEntryDto>.IndexKeys;

            var uniqueIndexModel = new CreateIndexModel<UserEmoteAnnotationEntryDto>(
                    build.Ascending(x => x.UserId),
                    new CreateIndexOptions() { Name = name, Unique = true, Background = false });

            col.Indexes.CreateOne(uniqueIndexModel);
        }

        private void CreateTtlIndex(string name, IMongoCollection<UserEmoteAnnotationEntryDto> col)
        {
            var expiresAfter = TimeSpan.FromSeconds(1);

            var build = Builders<UserEmoteAnnotationEntryDto>.IndexKeys;

            var ttlIndexModel = new CreateIndexModel<UserEmoteAnnotationEntryDto>(
                    build.Ascending(x => x.Expiry),
                    new CreateIndexOptions() { Name = name, Unique = false, ExpireAfter = expiresAfter, Background = false });

            col.Indexes.CreateOne(ttlIndexModel);
        }

        private FilterDefinition<UserEmoteAnnotationEntryDto> CreateEqualityFilter(ulong userId)
            => Builders<UserEmoteAnnotationEntryDto>.Filter.Eq(us => us.UserId, userId);
    }
}
