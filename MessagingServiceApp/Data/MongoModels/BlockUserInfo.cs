using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MessagingServiceApp.Data.MongoModels
{
    public class BlockUserInfo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string BlockingUserId { get; set; }

        public string BlockingUserEmail { get; set; }

        public string BlockingUserUserName { get; set; }

        public string BlockedUserId { get; set; }

        public string BlockedUserEmail { get; set; }

        public string BlockedUserUserName { get; set; }

        public bool IsBlocked { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}