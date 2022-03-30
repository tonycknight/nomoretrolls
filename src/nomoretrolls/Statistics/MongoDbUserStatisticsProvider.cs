﻿using System.Diagnostics.CodeAnalysis;
using MongoDB.Bson;
using MongoDB.Driver;
using nomoretrolls.Config;
using nomoretrolls.Io;

namespace nomoretrolls.Statistics
{
    [ExcludeFromCodeCoverage]
    internal class MongoDbUserStatisticsProvider : IUserStatisticsProvider
    {
        private readonly Lazy<IMongoCollection<UserStatisticsEntryDto>> _statsCol;

        public MongoDbUserStatisticsProvider(IConfigurationProvider config)
        {
            _statsCol = new Lazy<IMongoCollection<UserStatisticsEntryDto>>(() => InitialiseDb(config));
        }

        public Task BumpUserStatisticAsync(ulong userId, string statName)
        {            
            var col = _statsCol.Value;
                        
            var now = DateTime.UtcNow;            
            var time = new TimeSpan(now.TimeOfDay.Hours, now.TimeOfDay.Minutes, 0);
            var timeFrame = now.Date + time;

            var filter = CreateEqualityFilter(userId, statName, timeFrame);
            
            var update = Builders<UserStatisticsEntryDto>.Update.Inc(us => us.Count, 1);

            return col.UpdateOneAsync(filter, update, new UpdateOptions() { IsUpsert = true });
        }

        
        public async Task<long> GetUserStatisticCountAsync(ulong userId, string statName, TimeSpan timeFrame)
        {            
            var col = _statsCol.Value;

            var from = DateTime.UtcNow.Add(-timeFrame);
            var filter = CreateQueryFilter(userId, statName, from);            
            var opts = new FindOptions<UserStatisticsEntryDto, BsonDocument>()
            {
                Projection = Builders<UserStatisticsEntryDto>.Projection.Include(u => u.Count).Exclude(u => u.Id),
            };

            var ys = await col.FindAsync(filter, opts);
            
            var sum = 0L;
            
            await ys.ForEachAsync(bd =>
            {
                var c = bd["count"].ToInt64();
                sum += c;
            });

            return sum;
        }

        private FilterDefinition<UserStatisticsEntryDto> CreateEqualityFilter(ulong userId, string statName, DateTime timeFrame)        
            => Builders<UserStatisticsEntryDto>.Filter.And(
                            Builders<UserStatisticsEntryDto>.Filter.Eq(us => us.UserId, userId),
                            Builders<UserStatisticsEntryDto>.Filter.Eq(us => us.Name, statName),
                            Builders<UserStatisticsEntryDto>.Filter.Eq(us => us.Time, timeFrame) );

        private FilterDefinition<UserStatisticsEntryDto> CreateQueryFilter(ulong userId, string statName, DateTime timeFrame)
            => Builders<UserStatisticsEntryDto>.Filter.And(
                            Builders<UserStatisticsEntryDto>.Filter.Eq(us => us.UserId, userId),
                            Builders<UserStatisticsEntryDto>.Filter.Eq(us => us.Name, statName),
                            Builders<UserStatisticsEntryDto>.Filter.Gte(us => us.Time, timeFrame));

        private IMongoCollection<UserStatisticsEntryDto> InitialiseDb(IConfigurationProvider configProvider)
        {
            var config = configProvider.GetValidateConfig();

            var db = config.MongoDb.GetDb();

            return CreateUserStatsCollection(config.MongoDb, db);
        }

        private IMongoCollection<UserStatisticsEntryDto> CreateUserStatsCollection(MongoDbConfiguration config, IMongoDatabase db)
        {

            var col = db.GetCollection<UserStatisticsEntryDto>(config.UserStatsCollectionName);
            if (col == null)
            {
                var opts = new CreateCollectionOptions();
                db.CreateCollection(config.UserStatsCollectionName, opts);
            }            
            col = db.GetCollection<UserStatisticsEntryDto>(config.UserStatsCollectionName);

            col.RecreateIndex("unique", (n, c) => CreateUniqueIndex(n, col));
            col.RecreateIndex("ttl", (n, c) => CreateTtlIndex(n, col));

            return col;
        }

        private void CreateUniqueIndex(string name, IMongoCollection<UserStatisticsEntryDto> col)
        {
            var build = Builders<UserStatisticsEntryDto>.IndexKeys;

            var uniqueIndexModel = new CreateIndexModel<UserStatisticsEntryDto>(
                    build.Ascending(x => x.UserId)
                         .Ascending(x => x.Name)
                         .Ascending(x => x.Time),
                    new CreateIndexOptions() { Name = name, Unique = true, Background = false });
            
            col.Indexes.CreateOne(uniqueIndexModel);
        }
        
        private void CreateTtlIndex(string name, IMongoCollection<UserStatisticsEntryDto> col)
        {
            var expiresAfter = TimeSpan.FromHours(48);

            var build = Builders<UserStatisticsEntryDto>.IndexKeys;            

            var ttlIndexModel = new CreateIndexModel<UserStatisticsEntryDto>(
                    build.Ascending(x => x.Time),
                    new CreateIndexOptions() { Name = name, Unique = false, ExpireAfter = expiresAfter, Background = false });
            
            col.Indexes.CreateOne(ttlIndexModel);
        }
    }
}
