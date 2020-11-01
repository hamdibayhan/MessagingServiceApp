using System;
using AutoMapper;
using MongoDB.Driver;
using MessagingServiceApp.Data.Entity;
using MessagingServiceApp.Dto.ApiResponse;
using MessagingServiceApp.Interfaces;
using MessagingServiceApp.Data.MongoModels;
using MessagingServiceApp.Data.Interfaces;
using static MessagingServiceApp.Data.Cache.Redis.RedisDb;

namespace MessagingServiceApp.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<BlockUserInfo> blockUserCollection;
        private readonly IMapper mapper;
        private readonly ICacheService cacheService;

        public UserService(
            IMongoDatabaseSettings settings,
            IUserProviderService userProvider,
            IMapper mapper,
            ICacheService cacheService)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            blockUserCollection = database.GetCollection<BlockUserInfo>(settings.BlockUsersCollectionName);
            this.mapper = mapper;
            this.cacheService = cacheService;
        }

        public BlockUserResponse BlockUser(User blockingUser, User blockedUser)
        {
            var insertBlockUserInfo = GetBlockUserInfo(blockingUser, blockedUser);
            InsertBlockUserInfoToMongo(insertBlockUserInfo);
            SetBlockUserToRedis(blockingUser, blockedUser);

            return GetResponseBlockUserInfo(insertBlockUserInfo);
        }

        public bool IsUserAlreadyBlocked(User blockingUser, User blockedUser)
        {
            var isBlockingUserExist = cacheService.GetServiceDb((int)RedisDbNumbers.BlockUser).
                                                   HashGetValue(blockingUser.Email,
                                                                blockedUser.Email);

            if (isBlockingUserExist.HasValue && (bool)isBlockingUserExist == true)
                return true;

            return false;
        }

        private void InsertBlockUserInfoToMongo(BlockUserInfo blockUserInfo) =>
            blockUserCollection.InsertOne(blockUserInfo);

        private void SetBlockUserToRedis(User blockingUser, User blockedUser) =>
            cacheService.GetServiceDb((int)RedisDbNumbers.BlockUser)
                        .HashSet(blockingUser.Email, blockedUser.Email, true);

        private BlockUserInfo GetBlockUserInfo(User blockingUser, User blockedUser)
        {
            return new BlockUserInfo()
            {
                BlockingUserId = blockingUser.Id,
                BlockingUserEmail = blockingUser.Email,
                BlockingUserUserName = blockingUser.UserName,
                BlockedUserId = blockedUser.Id,
                BlockedUserEmail = blockedUser.Email,
                BlockedUserUserName = blockedUser.UserName,
                IsBlocked = true,
                CreatedAt = DateTime.Now,
            };
        }

        private BlockUserResponse GetResponseBlockUserInfo(BlockUserInfo blockUserInfo)
        {
            return mapper.Map<BlockUserResponse>(blockUserInfo);
        }
    }
}
