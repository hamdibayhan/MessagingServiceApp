using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MessagingServiceApp.Dto.ApiParameter;
using MessagingServiceApp.Interfaces;
using System.Net.Mime;
using MessagingServiceApp.Dto.ApiResponse;
using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using MessagingServiceApp.Libraries;

namespace MessagingServiceApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService messageService;
        private readonly IUserProviderService userProvider;
        private readonly IUserService userService;
        private readonly ILogger<MessageController> logger;

        public MessageController(
            IMessageService messageService,
            IUserProviderService userProvider,
            IUserService userService,
            ILogger<MessageController> logger)
        {
            this.messageService = messageService;
            this.userProvider = userProvider;
            this.userService = userService;
            this.logger = logger;
        }

        // POST api/message/sendMessage
        [HttpPost("sendMessage")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ServiceFilter(typeof(UserActivityLoggingActionFilter))]
        public async Task<IActionResult> SendMessageAsync([FromBody] SendMessageParams messageInfo)
        {
            try
            {
                var contactUser = await userProvider.GetUserWithUserNameAsync(messageInfo.ContactUserName);
                var senderUser = await userProvider.GetCurrentUser();

                if (contactUser == null)
                    return NotFound(Response<string>.GetError(null, $"There is no user with name: '{messageInfo.ContactUserName}'"));

                var isUserBlocked = userService.IsUserAlreadyBlocked(contactUser, senderUser);
                if (isUserBlocked)
                    return BadRequest(Response<string>.GetError(null, "User restricted for sending message to contact user"));

                var result = messageService.CreateMessageInfo(messageInfo, senderUser, contactUser);
                if (result != null)
                    return Ok(Response<CreateMessageInfoResponse>.GetSuccess(result));
                else
                    return BadRequest(Response<string>.GetError(null, "Message could not send"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return StatusCode(500, Response<string>.GetError(null, "An error occured"));
            }
        }

        // POST api/message/messageList
        [HttpGet("messageList")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ServiceFilter(typeof(UserActivityLoggingActionFilter))]
        public async Task<IActionResult> MessageListAsync([FromBody] MessageListParams messageList)
        {
            try
            {
                var senderUser = await userProvider.GetCurrentUser();
                var contactUser = await userProvider.GetUserWithUserNameAsync(messageList.ContactUserUserName);

                if (contactUser == null)
                    return NotFound(Response<string>.GetError(null, $"There is no user with name: '{messageList.ContactUserUserName}'"));

                var data = await messageService.GetMessageInfoListAsync(messageList,
                                                                        senderUser, 
                                                                        contactUser);
                if (data != null)
                    return Ok(Response<List<MessageInfoResponse>>.GetSuccess(data));
                else
                    return BadRequest(Response<string>.GetError(null, "Message list could not get"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                return StatusCode(500, Response<string>.GetError(null, "An error occured"));
            }
        }
    }
}