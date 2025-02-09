namespace Gateway.Services
{
    public interface IConnectionClient
    {
        public abstract HttpClient getClient();
    }

    public class ConnectionClient: IConnectionClient
    {

        private readonly HttpClient _client;

        public ConnectionClient() {
            _client = new HttpClient();
        }

        public HttpClient getClient()
        {
            return this._client;
        }
    }
}
