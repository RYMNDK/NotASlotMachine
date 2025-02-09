using boxs.Models;

namespace boxs.Services
{
    public interface IBalanceClient
    {
        public abstract Task<long> PayAsync(BetDTO betDTO);
        public abstract Task<long> UpdateBalanceAsync(PlayerBalanceDTO playerBalance);
    }

    public class BalanceClient : IBalanceClient
    {
        private readonly HttpClient _client;
        private readonly String _endPoint;

        public BalanceClient()
        {
            _client = new HttpClient();
            _endPoint = "http://localhost:8082/Balance";
        }

        public async Task<long> PayAsync(BetDTO betDTO)
        {
            try
            {
                // BetDTO is Mapped to PlayerBetDTO in Balance
                var response = await _client.PostAsJsonAsync(_endPoint, betDTO);
                response.EnsureSuccessStatusCode();

                var playerBalance = await response.Content.ReadFromJsonAsync<PlayerBalanceDTO>();
                return playerBalance!.balance;
            }
            catch (Exception)
            {
                return -1;
            }

        }

        public async Task<long> UpdateBalanceAsync(PlayerBalanceDTO playerBalance)
        {
            try
            {
                var response = await _client.PutAsJsonAsync(_endPoint, playerBalance);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<PlayerBalanceDTO>();
                return result!.balance;
            }
            catch (Exception)
            {
                return -1;
            }

        }
    }
}

