using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MessagingServiceApp.Data.MongoModel
{
    public class MessageInfo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string SenderUserId { get; set; }

        public string SenderUserEmail { get; set; }

        public string ContactUserId { get; set; }

        public string ContactUserEmail { get; set; }

        public string MessageText { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}