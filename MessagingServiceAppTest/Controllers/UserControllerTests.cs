using System;
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
    public class UserControllerTests
    {
        private readonly Mock<ILogger<UserController>> logger;
        private readonly Mock<IUserService> userService;
        private readonly Mock<IUserProviderService> userProviderService;

        private List<User> _users = new List<User>
        {
                new User() { Id = "123", UserName = "User1", Email = "user1@bv.com", PasswordHash = "P@ssw0rd" },
                new User() { Id = "124", UserName = "User2", Email = "user2@bv.com", PasswordHash = "P@ssw0rd" }
        };

        public UserControllerTests()
        {
            userService = new Mock<IUserService>();
            userProviderService = new Mock<IUserProviderService>();
            logger = new Mock<ILogger<UserController>>();
        }

        [Fact]
        public async Task BlockUser_ValidProcess_BlockTheContactUser()
        {
            var blockUserParams = Mock.Of<BlockUserParams>();
            blockUserParams.UserEmail = _users[1].Email;
            var blockUserResponse = Mock.Of<BlockUserResponse>();

            // arrange
            userProviderService.Setup(x => x.GetUserWithEmailAsync(_users[1].Email)).Returns(Task.FromResult(_users[1]));
            userProviderService.Setup(x => x.GetCurrentUser()).Returns(Task.FromResult(_users[1]));
            userService.Setup(x => x.IsUserAlreadyBlocked(_users[1], _users[1])).Returns(false);
            userService.Setup(x => x.BlockUser(_users[1], _users[1])).Returns(blockUserResponse);

            var controller = new UserController(userService.Object, userProviderService.Object, logger.Object);

            // act
            var result = await controller.BlockUserAsync(blockUserParams);
            var objResult = result as ObjectResult;

            // assert
            Assert.NotNull(objResult);
            Assert.True(objResult is OkObjectResult);
            Assert.Equal(StatusCodes.Status200OK, objResult.StatusCode);
        }

        [Fact]
        public async Task BlockUser_NullBlockedUser_BlockedUserMustBeExist()
        {
            var blockUserParams = Mock.Of<BlockUserParams>();

            // arrange
            userProviderService.Setup(x => x.GetUserWithEmailAsync(_users[1].Email)).Returns(Task.FromResult((User)null));

            var controller = new UserController(userService.Object, userProviderService.Object, logger.Object);

            // act
            var result = await controller.BlockUserAsync(blockUserParams);
            var objResult = result as ObjectResult;

            // assert
            Assert.NotNull(objResult);
            Assert.True(objResult is NotFoundObjectResult);
            Assert.Equal(StatusCodes.Status404NotFound, objResult.StatusCode);
        }

        [Fact]
        public async Task BlockUser_UserAlreadyBlocked_UserMustBeNotBlocked()
        {
            var blockUserParams = Mock.Of<BlockUserParams>();
            blockUserParams.UserEmail = _users[1].Email;

            // arrange
            userProviderService.Setup(x => x.GetUserWithEmailAsync(_users[1].Email)).Returns(Task.FromResult(_users[1]));
            userProviderService.Setup(x => x.GetCurrentUser()).Returns(Task.FromResult(_users[1]));
            userService.Setup(x => x.IsUserAlreadyBlocked(_users[1], _users[1])).Returns(true);

            var controller = new UserController(userService.Object, userProviderService.Object, logger.Object);

            // act
            var result = await controller.BlockUserAsync(blockUserParams);
            var objResult = result as ObjectResult;

            // assert
            Assert.NotNull(objResult);
            Assert.True(objResult is BadRequestObjectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objResult.StatusCode);
        }

        [Fact]
        public async Task BlockUser_UserBlockResultNull_UserBlockUserResultCannotBeNull()
        {
            var blockUserParams = Mock.Of<BlockUserParams>();
            blockUserParams.UserEmail = _users[1].Email;

            // arrange
            userProviderService.Setup(x => x.GetUserWithEmailAsync(_users[1].Email)).Returns(Task.FromResult(_users[1]));
            userProviderService.Setup(x => x.GetCurrentUser()).Returns(Task.FromResult(_users[1]));
            userService.Setup(x => x.IsUserAlreadyBlocked(_users[1], _users[1])).Returns(false);
            userService.Setup(x => x.BlockUser(_users[1], _users[1])).Returns((BlockUserResponse)null);

            var controller = new UserController(userService.Object, userProviderService.Object, logger.Object);

            // act
            var result = await controller.BlockUserAsync(blockUserParams);
            var objResult = result as ObjectResult;

            // assert
            Assert.NotNull(objResult);
            Assert.True(objResult is BadRequestObjectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objResult.StatusCode);
        }
    }
}
