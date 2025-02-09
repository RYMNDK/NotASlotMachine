using MongoDB.Driver;
using boxs.Models;

namespace boxs.Services
{
    public interface IMachineConfiguration
    {
        public abstract Task<boxMachine> GetConfigAsync();
        public abstract Task<bool> SetConfigAsync(boxMachineDTO newParameters);
    }

    public class MachineConfiguration : IMachineConfiguration
    {
        private readonly IMongoCollection<boxMachine> _collection;

        public MachineConfiguration(IConfiguration configuration)
        {
            string connectionUri = configuration.GetSection("MongoDB:ConnectionString").Value ?? 
                throw new ArgumentNullException("MongoDB ConnectionString is not configured.");
            string databaseName = configuration.GetSection("MongoDB:DatabaseName").Value ?? 
                throw new ArgumentNullException("MongoDB DatabaseName is not configured.");
            string collectionName = configuration.GetSection("MongoDB:Collection").Value ??
                throw new ArgumentNullException("MongoDB Collection is not configured.");

            var settings = MongoClientSettings.FromConnectionString(connectionUri);
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            _collection = new MongoClient(settings).GetDatabase(databaseName).GetCollection<boxMachine>(collectionName);

        }

        public async Task<boxMachine> GetConfigAsync()
        {
            var machineParams = await _collection.Find(_ => true).FirstOrDefaultAsync();
            return machineParams ?? throw new InvalidOperationException("MachineConfiguration is not set up.");
        }

        public async Task<bool> SetConfigAsync(boxMachineDTO newParameters)
        {
            var updateQuery = Builders<boxMachine>.Update
                .Set(param => param.maxX, newParameters.maxX)
                .Set(param => param.maxY, newParameters.maxY);
            var updateResult = await _collection.UpdateOneAsync(_ => true, updateQuery);
            return updateResult.IsAcknowledged && updateResult.MatchedCount > 0;
        }
    }
}
