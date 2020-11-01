using System;
using System.Net.Mime;
using System.Threading.Tasks;
using MessagingServiceApp.Dto.ApiParameter;
using MessagingServiceApp.Dto.ApiResponse;
using MessagingServiceApp.Interfaces;
using MessagingServiceApp.Libraries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MessagingServiceApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IUserProviderService userProvider;
        private readonly ILogger<UserController> logger;

        public UserController(
            IUserService userService,
            IUserProviderService userProvider,
            ILogger<UserController> logger)
        {
            this.userService = userService;
            this.userProvider = userProvider;
            this.logger = logger;
        }

        // POST api/user/blockUser
        [HttpPost("blockUser")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ServiceFilter(typeof(UserActivityLoggingActionFilter))]
        public async Task<IActionResult> BlockUserAsync([FromBody] BlockUserParams blockUserParams)
        {
            try
            {
                var blockingUser = await userProvider.GetCurrentUser();
                var blockedUser = await userProvider.GetUserWithEmailAsync(blockUserParams.UserEmail);

                if (blockedUser == null)
                    return NotFound(Response<string>.GetError(null, $"There is no user with email: '{blockUserParams.UserEmail}'"));

                var isUserAlreadyBlocked = userService.IsUserAlreadyBlocked(blockingUser, blockedUser);
                if (isUserAlreadyBlocked)
                    return BadRequest(Response<string>.GetError(null, $"User already blocked with email: '{blockUserParams.UserEmail}'"));

                var result = userService.BlockUser(blockingUser, blockedUser);
                if(result != null)
                    return Ok(Response<BlockUserResponse>.GetSuccess(result));
                else
                    return BadRequest(Response<string>.GetError(null, "User could not blocked"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return StatusCode(500, Response<string>.GetError(null, "An error occured"));
            }

        }
    }
}
