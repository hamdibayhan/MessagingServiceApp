using MessagingServiceApp.Data.Interfaces;
using StackExchange.Redis;

namespace MessagingServiceApp.Data.Cache.Redis
{
    public class RedisCacheService : ICacheService
    {
        private readonly RedisServer redisServer;
        private IDatabase database;

        public RedisCacheService(RedisServer redisServer)
        {
            this.redisServer = redisServer;
            database = redisServer.Database;
        }

        public void HashSet(RedisKey key, RedisValue value, RedisValue flag) =>
            database.HashSet(key, value, flag);

        public RedisValue HashGetValue(RedisKey key, RedisValue value)
        {
            return database.HashGet(key, value);
        }

        public ICacheService GetServiceDb(int dbNumber)
        {
            SetDB(dbNumber);
            return this;
        }

        private void SetDB(int dbNumber) =>
            database = redisServer.GetDatabase(dbNumber);
    }
}