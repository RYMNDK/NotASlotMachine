using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace boxs.Models
{
    public record boxMachine(int maxX, int maxY)
    {
        [BsonId]
        public ObjectId Id { get; set; }
    }   
    public record BetDTO(int playerId, long bet);

    public record WinDTO(int playerId, long win, long balance, String resultCSV);

    public record PlayerBalanceDTO(int playerid, long balance);

    public record boxMachineDTO(int maxX, int maxY);
}
