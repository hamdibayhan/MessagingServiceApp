﻿using System;
using AutoMapper;
using MessagingServiceApp.Data.Entity;
using MessagingServiceApp.Data.MongoModel;
using MessagingServiceApp.Dto.ApiResponse;
using MessagingServiceApp.Dto.ApiParameter;
using MessagingServiceApp.Interfaces;
using MongoDB.Driver;
using MessagingServiceApp.Data.Interfaces;

namespace MessagingServiceApp.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMongoCollection<MessageInfo> mongoMessagesCollection;
        private readonly IMapper mapper;

        public MessageService(IMongoDatabaseSettings settings,
            IUserProviderService userProvider,
            IMapper mapper)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            this.mongoMessagesCollection = database.GetCollection<MessageInfo>(settings.MessageInfosCollectionName);
            this.mapper = mapper;
        }

        public CreateMessageInfoResponse CreateMessageInfo(SendMessageParams message,
                                                           User senderUser, User contactUser)
        {
            var messageInfo = GetInsertMessageInfo(senderUser, contactUser, message);
            MessageInfoToMongo(messageInfo);

            return GetResponseMessageInfo(messageInfo);
        }

        private void MessageInfoToMongo(MessageInfo message) =>
            mongoMessagesCollection.InsertOne(message);

        private CreateMessageInfoResponse GetResponseMessageInfo(MessageInfo insertMessage)
        {
            return mapper.Map<CreateMessageInfoResponse>(insertMessage);
        }

        private MessageInfo GetInsertMessageInfo(User senderUser, User contactUser,
                                                 SendMessageParams message)
        {
            return new MessageInfo()
            {
                SenderUserId = senderUser.Id,
                SenderUserEmail = senderUser.Email,
                ContactUserId = contactUser.Id,
                ContactUserEmail = contactUser.Email,
                MessageText = message.MessageText,
                CreatedAt = DateTime.Now,
            };
        }
    }
}