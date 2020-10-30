using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace MessagingServiceApp.Data.Cache.Redis
{
    public class RedisServer
    {
        private ConnectionMultiplexer _connectionMultiplexer;
        private IDatabase _database;
        private string configurationString;

        public RedisServer(IConfiguration configuration)
        {
            SetRedisConfigurationString(configuration);
            _connectionMultiplexer = ConnectionMultiplexer.Connect(configurationString);
        }

        public IDatabase Database => _database;

        public IDatabase GetDatabase(int dbNumber)
        {
            return _connectionMultiplexer.GetDatabase(dbNumber);
        }

        private void SetRedisConfigurationString(IConfiguration configuration)
        {
            string host = configuration["RedisConfiguration:Host"];
            string port = configuration["RedisConfiguration:Port"];
            configurationString = $"{host}:{port}";
        }
    }
}