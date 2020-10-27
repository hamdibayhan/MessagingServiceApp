using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MessagingAppApiTest.Controllers;
using MessagingServiceApp.Data.Entity;
using MessagingServiceApp.Dto.ApiParameter;
using MessagingServiceApp.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace MessagingServiceAppTest.Controllers
{
    public class AccountControllerTests
    {
        private readonly Mock<FakeSignInManager> signInManager;
        private readonly Mock<ILogger<AccountController>> logger;
        private readonly Mock<UserManager<User>> userManager;
        private readonly Mock<IAccountService> accountService;

        private List<User> _users = new List<User>
        {
                new User() { Id = "123", UserName = "User1", Email = "user1@bv.com", PasswordHash = "P@ssw0rd" },
                new User() { Id = "124", UserName = "User2", Email = "user2@bv.com", PasswordHash = "P@ssw0rd" }
        };

        private const string _email = "test@test.com";

        public AccountControllerTests()
        {
            userManager = MockHelpers.MockUserManager(_users);
            signInManager = new Mock<FakeSignInManager>();
            accountService = new Mock<IAccountService>();
            logger = new Mock<ILogger<AccountController>>();
        }

        [Fact]
        public async Task Register_ValidProcess_CreateNewUserAsync()
        {
            var registerParams = Mock.Of<RegisterParams>();

            // arrange
            userManager.Setup(x => x.FindByEmailAsync(_email)).ReturnsAsync((User)null);
            accountService.Setup(x => x.CreateUser(registerParams)).Returns(IdentityResult.Success);
            var controller = new AccountController(logger.Object, signInManager.Object, userManager.Object, accountService.Object);

            // act
            var result = await controller.Register(registerParams);
            var objResult = result as ObjectResult;

            // assert
            Assert.NotNull(objResult);
            Assert.True(objResult is CreatedResult);
            Assert.Equal(StatusCodes.Status201Created, objResult.StatusCode);
        }

        [Fact]
        public async Task Register_AlreadyExistUser_CreateNewUserAsync()
        {
            var email = _users[1].Email;
            var registerParams = Mock.Of<RegisterParams>();
            registerParams.Email = email;

            // arrange
            userManager.Setup(x => x.FindByEmailAsync(email)).Returns(Task.FromResult(_users[1]));
            accountService.Setup(x => x.CreateUser(registerParams)).Returns(IdentityResult.Success);
            var controller = new AccountController(logger.Object, signInManager.Object, userManager.Object, accountService.Object);

            // act
            var result = await controller.Register(registerParams);
            var objResult = result as ObjectResult;

            // assert
            Assert.NotNull(objResult);
            Assert.True(objResult is BadRequestObjectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objResult.StatusCode);
        }

        [Fact]
        public async Task Register_FailedCreatingUser_CreateNewUserAsync()
        {
            var registerParams = Mock.Of<RegisterParams>();

            // arrange
            userManager.Setup(x => x.FindByEmailAsync(_email)).ReturnsAsync((User)null);
            accountService.Setup(x => x.CreateUser(registerParams)).Returns(IdentityResult.Failed());
            var controller = new AccountController(logger.Object, signInManager.Object, userManager.Object, accountService.Object);

            // act
            var result = await controller.Register(registerParams);
            var objResult = result as ObjectResult;

            // assert
            Assert.NotNull(objResult);
            Assert.True(objResult is BadRequestObjectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objResult.StatusCode);
        }

        [Fact]
        public async Task Login_ValidProcess_LoginUserAsync()
        {
            var email = _users[1].Email;
            var loginParams = Mock.Of<LoginParams>();
            loginParams.Email = email;

            var password = "P@ssw0rd";
            var _passwordHasher = new PasswordHasher<IdentityUser>();
            var hasedPassword = _passwordHasher.HashPassword(_users[1], password);
            _users[1].SecurityStamp = Guid.NewGuid().ToString();
            _users[1].PasswordHash = hasedPassword;

            // arrange
            userManager.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync(_users[1]);
            signInManager.Setup(x => x.CheckPasswordSignInAsync(_users[1], loginParams.Password, false)).ReturnsAsync(SignInResult.Success);
            accountService.Setup(x => x.GetLoginToken(It.IsAny<User>())).Returns(() => "token");

            var controller = new AccountController(logger.Object, signInManager.Object, userManager.Object, accountService.Object);

            // act
            var result = await controller.Login(loginParams);
            var objResult = result as ObjectResult;

            // assert
            Assert.NotNull(objResult);
            Assert.True(objResult is OkObjectResult);
            Assert.Equal(StatusCodes.Status200OK , objResult.StatusCode);
        }

        [Fact]
        public async Task Login_NoUser_LoginUserAsync()
        {
            var email = _users[1].Email;
            var loginParams = Mock.Of<LoginParams>();
            loginParams.Email = "no@user.com";

            // arrange
            userManager.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync(_users[1]);

            var controller = new AccountController(logger.Object, signInManager.Object, userManager.Object, accountService.Object);

            // act
            var result = await controller.Login(loginParams);
            var objResult = result as ObjectResult;

            // assert
            Assert.NotNull(objResult);
            Assert.True(objResult is NotFoundObjectResult);
            Assert.Equal(StatusCodes.Status404NotFound, objResult.StatusCode);
        }

        [Fact]
        public async Task Login_WrongPasswordLoginParameter_LoginUserAsync()
        {
            var email = _users[1].Email;
            var loginParams = Mock.Of<LoginParams>();
            loginParams.Email = email;

            var password = "P@ssw0rr";
            var _passwordHasher = new PasswordHasher<IdentityUser>();
            var hasedPassword = _passwordHasher.HashPassword(_users[1], password);
            _users[1].SecurityStamp = Guid.NewGuid().ToString();
            _users[1].PasswordHash = hasedPassword;

            // arrange
            userManager.Setup(x => x.FindByEmailAsync(email)).ReturnsAsync(_users[1]);
            signInManager.Setup(x => x.CheckPasswordSignInAsync(_users[1], loginParams.Password, false)).ReturnsAsync(SignInResult.Failed);

            var controller = new AccountController(logger.Object, signInManager.Object, userManager.Object, accountService.Object);

            // act
            var result = await controller.Login(loginParams);
            var objResult = result as ObjectResult;

            // assert
            Assert.NotNull(objResult);
            Assert.True(objResult is BadRequestObjectResult);
            Assert.Equal(StatusCodes.Status400BadRequest, objResult.StatusCode);
        }
    }
}
