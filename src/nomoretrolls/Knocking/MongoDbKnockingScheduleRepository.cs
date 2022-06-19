using System.Diagnostics.CodeAnalysis;
using MongoDB.Driver;
using nomoretrolls.Config;
using nomoretrolls.Io;
using nomoretrolls.Telemetry;

namespace nomoretrolls.Knocking
{
    [ExcludeFromCodeCoverage]
    internal class MongoDbKnockingScheduleRepository : IKnockingScheduleRepository
    {
        private readonly Lazy<IMongoCollection<KnockingScheduleEntryDto>> _knockScheduleCol;
        private readonly ITelemetry _telemetry;

        public MongoDbKnockingScheduleRepository(Config.IConfigurationProvider configProvider, ITelemetry telemetry)
        {
            _knockScheduleCol = new Lazy<IMongoCollection<KnockingScheduleEntryDto>>(() => InitialiseDb(configProvider));
            _telemetry = telemetry;
        }

        public async Task DeleteUserEntryAsync(ulong userId)
        {
            var filter = CreateEqualityFilter(userId);

            var col = _knockScheduleCol.Value;

            await col.DeleteOneAsync(filter);
        }

        public async Task<IList<KnockingScheduleEntry>> GetUserEntriesAsync()
        {
            var col = _knockScheduleCol.Value;
            var filter = Builders<KnockingScheduleEntryDto>.Filter.Gt(us => us.UserId, 0);

            var result = await col.FindAsync(filter);

            return result.ToList().Select(r => r.FromDto()).ToList();
        }

        public async Task SetUserEntryAsync(KnockingScheduleEntry entry)
        {
            var dto = entry.ToDto();

            var filter = CreateEqualityFilter(entry.UserId);

            var update = Builders<KnockingScheduleEntryDto>.Update
                .Set(us => us.UserId, dto.UserId)
                .Set(us => us.Start, dto.Start)
                .Set(us => us.Expiry, dto.Expiry)
                .Set(us => us.Frequency, dto.Frequency);

            var col = _knockScheduleCol.Value;

            var result = await col.UpdateOneAsync(filter, update, new UpdateOptions() { IsUpsert = true });
        }


        private IMongoCollection<KnockingScheduleEntryDto> InitialiseDb(IConfigurationProvider configProvider)
        {
            var config = configProvider.GetValidateConfig();

            var db = config.MongoDb.GetDb(_telemetry);

            return CreateScheduleCollection(config.MongoDb, db);
        }

        private IMongoCollection<KnockingScheduleEntryDto> CreateScheduleCollection(MongoDbConfiguration config, IMongoDatabase db)
        {
            var col = db.GetCollection<KnockingScheduleEntryDto>(config.UserKnockingScheduleCollectionName);
            if (col == null)
            {
                var opts = new CreateCollectionOptions();
                db.CreateCollection(config.UserKnockingScheduleCollectionName, opts);
            }
            col = db.GetCollection<KnockingScheduleEntryDto>(config.UserKnockingScheduleCollectionName);

            col.RecreateIndex("unique", (n, c) => CreateUniqueIndex(n, col));
            col.RecreateIndex("ttl", (n, c) => CreateTtlIndex(n, col));

            return col;
        }

        private void CreateUniqueIndex(string name, IMongoCollection<KnockingScheduleEntryDto> col)
        {
            var build = Builders<KnockingScheduleEntryDto>.IndexKeys;

            var uniqueIndexModel = new CreateIndexModel<KnockingScheduleEntryDto>(
                    build.Ascending(x => x.UserId),
                    new CreateIndexOptions() { Name = name, Unique = true, Background = false });

            col.Indexes.CreateOne(uniqueIndexModel);
        }

        private void CreateTtlIndex(string name, IMongoCollection<KnockingScheduleEntryDto> col)
        {
            var expiresAfter = TimeSpan.FromSeconds(1);

            var build = Builders<KnockingScheduleEntryDto>.IndexKeys;

            var ttlIndexModel = new CreateIndexModel<KnockingScheduleEntryDto>(
                    build.Ascending(x => x.Expiry),
                    new CreateIndexOptions() { Name = name, Unique = false, ExpireAfter = expiresAfter, Background = false });

            col.Indexes.CreateOne(ttlIndexModel);
        }

        private FilterDefinition<KnockingScheduleEntryDto> CreateEqualityFilter(ulong userId)
            => Builders<KnockingScheduleEntryDto>.Filter.Eq(us => us.UserId, userId);
    }
}
