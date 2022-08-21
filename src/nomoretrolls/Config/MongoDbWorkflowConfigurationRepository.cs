using System.Diagnostics.CodeAnalysis;
using MongoDB.Driver;
using nomoretrolls.Io;
using nomoretrolls.Telemetry;

namespace nomoretrolls.Config
{
    [ExcludeFromCodeCoverage]
    internal class MongoDbWorkflowConfigurationRepository : IWorkflowConfigurationRepository
    {
        private readonly Lazy<IMongoCollection<WorkflowConfigurationDto>> _configCol;
        private readonly ITelemetry _telemetry;

        public MongoDbWorkflowConfigurationRepository(Config.IConfigurationProvider configProvider, ITelemetry telemetry)
        {
            _configCol = new Lazy<IMongoCollection<WorkflowConfigurationDto>>(() => InitialiseDb(configProvider));
            _telemetry = telemetry;
        }

        public async Task<IList<WorkflowConfiguration>> GetWorkflowConfigsAsync()
        {

            var filter = Builders<WorkflowConfigurationDto>.Filter.Ne(us => us.Name, "");

            var col = _configCol.Value;

            var dtos = (await col.FindAsync(filter));

            var configs = dtos.ToEnumerable()
                              .ToDictionary(dto => dto.Name, dto => dto.FromDto(), StringComparer.InvariantCultureIgnoreCase);

            var names = new[] {
                IWorkflowConfigurationRepository.ShoutingWorkflow,
                IWorkflowConfigurationRepository.BlacklistWorkflow,
                IWorkflowConfigurationRepository.EmoteAnnotationWorkflow,
                IWorkflowConfigurationRepository.AltCapsWorkflow,
                IWorkflowConfigurationRepository.KnockingWorkflow
            };

            foreach(var n in names)
            {
                if (!configs.ContainsKey(n))
                {                    
                    var config = new WorkflowConfiguration() { Name = n, Enabled = true };
                    configs[n] = config;
                }
            }

            return configs.Values.ToList();
        }

        public async Task<WorkflowConfiguration> GetWorkflowConfigAsync(string name)
        {
            var filter = CreateEqualityFilter(name);

            var col = _configCol.Value;

            var result = (await col.FindAsync(filter)).FirstOrDefault();

            result = result ?? new WorkflowConfigurationDto() {  Name = name, Enabled = true };
                        
            return result.FromDto();
        }

        public async Task SetWorkflowConfigAsync(WorkflowConfiguration config)
        {
            var dto = config.ToDto();
            var filter = CreateEqualityFilter(dto.Name);

            var update = Builders<WorkflowConfigurationDto>.Update.Set(us => us.Name, dto.Name)
                .Set(us => us.Enabled, dto.Enabled);

            var col = _configCol.Value;

            var result = await col.UpdateOneAsync(filter, update, new UpdateOptions() { IsUpsert = true });
        }


        private IMongoCollection<WorkflowConfigurationDto> InitialiseDb(IConfigurationProvider configProvider)
        {
            var config = configProvider.GetValidateConfig();

            var db = config.MongoDb.GetDb(_telemetry);

            return CreateWorkflowConfigCollection(config.MongoDb, db);
        }

        private IMongoCollection<WorkflowConfigurationDto> CreateWorkflowConfigCollection(MongoDbConfiguration config, IMongoDatabase db)
        {
            var col = db.GetCollection<WorkflowConfigurationDto>(config.WorkflowConfigCollectionName);
            if (col == null)
            {
                var opts = new CreateCollectionOptions();
                db.CreateCollection(config.UserBlacklistCollectionName, opts);
            }
            col = db.GetCollection<WorkflowConfigurationDto>(config.WorkflowConfigCollectionName);

            col.RecreateIndex("unique", (n, c) => CreateUniqueIndex(n, col));
            
            return col;
        }

        private void CreateUniqueIndex(string name, IMongoCollection<WorkflowConfigurationDto> col)
        {
            var build = Builders<WorkflowConfigurationDto>.IndexKeys;

            var uniqueIndexModel = new CreateIndexModel<WorkflowConfigurationDto>(
                    build.Ascending(x => x.Name),
                    new CreateIndexOptions() { Name = name, Unique = true, Background = false });

            col.Indexes.CreateOne(uniqueIndexModel);
        }

        private FilterDefinition<WorkflowConfigurationDto> CreateEqualityFilter(string name)
            => Builders<WorkflowConfigurationDto>.Filter.Eq(us => us.Name, name);
    }
}
