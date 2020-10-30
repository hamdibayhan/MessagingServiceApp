using System.Collections.Generic;
using StackExchange.Redis;

namespace MessagingServiceApp.Data.Interfaces
{
    public interface ICacheService
    {
        void HashSet(RedisKey key, RedisValue value, RedisValue flag);
        RedisValue HashGetValue(RedisKey key, RedisValue value);
        ICacheService GetServiceDb(int dbNumber);
    }
}