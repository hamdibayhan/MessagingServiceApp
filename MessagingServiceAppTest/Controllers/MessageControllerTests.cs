﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MessagingServiceApp.Controllers;
using MessagingServiceApp.Data.Entity;
using MessagingServiceApp.Dto.ApiParameter;
using MessagingServiceApp.Dto.ApiResponse;
using MessagingServiceApp.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MessagingServiceAppTest.Controllers
{
    public class MessageControllerTests
    {
        private readonly Mock<ILogger<MessageController>> logger;
        private readonly Mock<IMessageService> messageService;
        private readonly Mock<IUserProviderService> userProviderService;
        private readonly Mock<IUserService> userService;

        private List<User> _users = new List<User>
        {
                new User() { Id = "123", UserName = "User1", Email = "user1@bv.com", PasswordHash = "P@ssw0rd" },
                new User() { Id = "124", UserName = "User2", Email = "user2@bv.com", PasswordHash = "P@ssw0rd" }
        };

        public MessageControllerTests()
        {
            messageService = new Mock<IMessageService>();
            userProviderService = new Mock<IUserProviderService>();
            userService = new Mock<IUserService>();
            logger = new Mock<ILogger<MessageController>>();
        }

        [Fact]
        public async Task SendMessage_ValidProcess_SendingMessageToContectUser()
        {
            var sendMessageParams = Mock.Of<SendMessageParams>();
            sendMessageParams.ContactUserName = _users[1].UserName;
            var createMessageInfoResponse = Mock.Of<CreateMessageInfoResponse>();

            // arrange
            userProviderService.Setup(x => x.GetUserWithUserNameAsync(_users[1].UserName)).Returns(Task.FromResult(_users[1]));
            userProviderService.Setup(x => x.GetCurrentUser()).Returns(Task.FromResult(_users[1]));
            userService.Setup(x => x.IsUserAlreadyBlocked(_users[1], _users[1])).Returns(false);
            messageService.Setup(x => x.CreateMessageInfo(sendMessageParams, _users[1], _users[1])).Returns(createMessageInfoResponse);

            var controller = new MessageController(messageService.Object, userProviderService.Object, userService.Object, logger.Object);

            // act
            var result = await controller.SendMessageAsync(sendMessageParams);
            var objResult = result as ObjectResult;

            // assert
            Assert.NotNull(objResult);
            Assert.True(objResult is OkObjectResult);
            Assert.Equal(StatusCodes.Status200OK, objResult.StatusCode);
        }

        [Fact]
        public async Task SendMessage_NullContactUser_ContactUserMustBeExist()
        {
            var sendMessageParams = Mock.Of<SendMessageParams>();

            // arrange
            userProviderService.Setup(x => x.GetUserWithUserNameAsync(_users[1].UserName)).Returns(Task.FromResult((User)null));

            var controller = new MessageController(messageService.Object, userProviderService.Object, userService.Object, logger.Object);

            // act
            var result = await controller.SendMessageAsync(sendMessageParams);
            var objResult = result as ObjectResult;

            // assert
            Assert.NotNull(objResult);
            Assert.True(objResult is NotFoundObjectResult);
            Assert.Equal(StatusCodes.Status404NotFound, objResult.StatusCode);
        }

        [Fact]
        public async Task SendMessage_NullInsertMongoResult_InsertMongoResultCannotBeNull()
        {
            var sendMessageParams = Mock.Of<SendMessageParams>();
            sendMessageParams.ContactUserName = _users[1].UserName;

            // arrange
            userProviderService.Setup(x => x.GetUserWithUserNameAsync(_users[1].UserName)).Returns(Task.FromResult(_users[1]));
            userProviderService.Setup(x => x.GetCurrentUser()).Returns(Task.FromResult(_users[1]));
            userService.Setup(x => x.IsUserAlreadyBlocked(_users[1], _users[1])).Returns(false);
            messageService.Setup(x => x.CreateMessageInfo(sendMessageParams, _users[1], _users[1])).Returns((CreateMessageInfoResponse)null);

            var controller = new MessageController(messageService.Object, userProviderService.Object, userService.Object, logger.Object);

            // act
            var result = await controller.SendMessageAsync(sendMessageParams);
            var objResult = result as ObjectResult;

            // assert
            Assert.NotNull(objResult);
            Assert.True(objResult is BadRequestObjectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objResult.StatusCode);
        }


        [Fact]
        public async Task SendMessage_CheckUserBlockStatus_MustNotBeBlockStatusForSendingMessage()
        {
            var sendMessageParams = Mock.Of<SendMessageParams>();
            sendMessageParams.ContactUserName = _users[1].UserName;

            // arrange
            userProviderService.Setup(x => x.GetUserWithUserNameAsync(_users[1].UserName)).Returns(Task.FromResult(_users[1]));
            userProviderService.Setup(x => x.GetCurrentUser()).Returns(Task.FromResult(_users[1]));
            userService.Setup(x => x.IsUserAlreadyBlocked(_users[1], _users[1])).Returns(true);

            var controller = new MessageController(messageService.Object, userProviderService.Object, userService.Object, logger.Object);

            // act
            var result = await controller.SendMessageAsync(sendMessageParams);
            var objResult = result as ObjectResult;

            // assert
            Assert.NotNull(objResult);
            Assert.True(objResult is BadRequestObjectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objResult.StatusCode);
        }

        [Fact]
        public async Task MessageList_ValidProcess_SendingMessageToContectUser()
        {
            var listMessageParams = Mock.Of<MessageListParams>();
            listMessageParams.ContactUserUserName = _users[1].UserName;
            var messageInfoResponseList = Mock.Of<List<MessageInfoResponse>>();

            // arrange
            userProviderService.Setup(x => x.GetUserWithUserNameAsync(_users[1].UserName)).Returns(Task.FromResult(_users[1]));
            userProviderService.Setup(x => x.GetCurrentUser()).Returns(Task.FromResult(_users[1]));
            messageService.Setup(x => x.GetMessageInfoListAsync(listMessageParams, _users[1], _users[1])).Returns(Task.FromResult(messageInfoResponseList));

            var controller = new MessageController(messageService.Object, userProviderService.Object, userService.Object, logger.Object);

            // act
            var result = await controller.MessageListAsync(listMessageParams);
            var objResult = result as ObjectResult;

            // assert
            Assert.NotNull(objResult);
            Assert.True(objResult is OkObjectResult);
            Assert.Equal(StatusCodes.Status200OK, objResult.StatusCode);
        }

        [Fact]
        public async Task MessageList_NullContactUser_ContactUserMustBeExist()
        {
            var listMessageParams = Mock.Of<MessageListParams>();

            // arrange
            userProviderService.Setup(x => x.GetUserWithUserNameAsync(_users[1].UserName)).Returns(Task.FromResult((User)null));

            var controller = new MessageController(messageService.Object, userProviderService.Object, userService.Object, logger.Object);

            // act
            var result = await controller.MessageListAsync(listMessageParams);
            var objResult = result as ObjectResult;

            // assert
            Assert.NotNull(objResult);
            Assert.True(objResult is NotFoundObjectResult);
            Assert.Equal(StatusCodes.Status404NotFound, objResult.StatusCode);
        }

        [Fact]
        public async Task MessageList_NullMessageInfoList_MessageInfoListResultModelCannotBeNull()
        {
            var listMessageParams = Mock.Of<MessageListParams>();
            listMessageParams.ContactUserUserName = _users[1].UserName;

            // arrange
            userProviderService.Setup(x => x.GetUserWithUserNameAsync(_users[1].UserName)).Returns(Task.FromResult(_users[1]));
            userProviderService.Setup(x => x.GetCurrentUser()).Returns(Task.FromResult(_users[1]));
            messageService.Setup(x => x.GetMessageInfoListAsync(listMessageParams, _users[1], _users[1])).Returns(Task.FromResult((List<MessageInfoResponse>)null));

            var controller = new MessageController(messageService.Object, userProviderService.Object, userService.Object, logger.Object);

            // act
            var result = await controller.MessageListAsync(listMessageParams);
            var objResult = result as ObjectResult;

            // assert
            Assert.NotNull(objResult);
            Assert.True(objResult is BadRequestObjectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objResult.StatusCode);
        }
    }
}
