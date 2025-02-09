
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Balance.Models
{
    public record PlayerBalance(int playerid, long balance) {
        [BsonId]
        public ObjectId Id { get; set; }
    }

    public record PlayerBalanceDTO(int playerid, long balance);

    public record PlayerBetDTO(int playerid, long bet);
}
