using Balance.Models;
using MongoDB.Driver;

namespace Balance.Services
{
    public interface IBalanceManager
    {
        public abstract Task<PlayerBalance> UpdateBalance(int playerId, long amount);
        public abstract Task<long> DeductBalance(int playerId, long amount);
    }

    public class BalanceManager : IBalanceManager
    {
        private readonly IMongoCollection<PlayerBalance> _collection;

        public BalanceManager(IConfiguration configuration)
        {
            string connectionUri = configuration.GetSection("MongoDB:ConnectionString").Value ?? 
                throw new ArgumentNullException("MongoDB ConnectionString is not configured.");
            string databaseName = configuration.GetSection("MongoDB:DatabaseName").Value ?? 
                throw new ArgumentNullException("MongoDB DatabaseName is not configured.");
            string collectionName = configuration.GetSection("MongoDB:Collection").Value ??
                throw new ArgumentNullException("MongoDB Collection is not configured.");

            var settings = MongoClientSettings.FromConnectionString(connectionUri);
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            _collection = new MongoClient(settings).GetDatabase(databaseName).GetCollection<PlayerBalance>(collectionName);

        }

        public async Task<PlayerBalance> UpdateBalance(int playerId, long amount)
        {
            var filter = Builders<PlayerBalance>.Filter.Where(account => account.playerid == playerId);
            var UpdateQuery = Builders<PlayerBalance>.Update.Inc(account => account.balance, amount);
            var options = new FindOneAndUpdateOptions<PlayerBalance>
            {
                ReturnDocument = ReturnDocument.After
            };
            var updatedAccount = await _collection.FindOneAndUpdateAsync(filter, UpdateQuery, options);

            return (updatedAccount ?? throw new InvalidOperationException("User is not found."));
        }

        public async Task<long> DeductBalance(int playerId, long amount)
        {
            var filter = Builders<PlayerBalance>.Filter.Where( account => account.playerid == playerId && account.balance >= amount);
            var UpdateQuery = Builders<PlayerBalance>.Update.Inc(account => account.balance, -amount);
            var options = new FindOneAndUpdateOptions<PlayerBalance>
            {
                ReturnDocument = ReturnDocument.After
            };
            var updatedAccount = await _collection.FindOneAndUpdateAsync(filter, UpdateQuery, options);

            return (updatedAccount != null ? updatedAccount.balance : -1);
        }

    }
}
