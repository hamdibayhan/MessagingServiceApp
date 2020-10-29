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

namespace MessagingServiceApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService messageService;
        private readonly IUserProviderService userProvider;
        private readonly ILogger<MessageController> logger;

        public MessageController(IMessageService messageService,
            IUserProviderService userProvider,
            ILogger<MessageController> logger)
        {
            this.messageService = messageService;
            this.userProvider = userProvider;
            this.logger = logger;
        }

        // POST api/message/sendMessage
        [HttpPost("sendMessage")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> SendMessageAsync([FromBody] SendMessageParams messageInfo)
        {
            try
            {
                var contactUser = await userProvider.GetUserWithUserNameAsync(messageInfo.ContactUserName);
                var senderUser = await userProvider.GetCurrentUser();

                if (contactUser == null)
                    return NotFound(Response<string>.GetError(null, $"There is no user with name: '{messageInfo.ContactUserName}'"));

                var result = messageService.CreateMessageInfo(messageInfo, senderUser, contactUser);
                if (result != null)
                    return Ok(Response<CreateMessageInfoResponse>.GetSuccess(result));
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
            }
            return BadRequest(Response<string>.GetError(null, "An error occured"));
        }

        // POST api/message/messageList
        [HttpGet("messageList")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        public async Task<IActionResult> MessageListAsync([FromBody] MessageListParams messageList)
        {
            try
            {
                var senderUser = await userProvider.GetCurrentUser();
                var contactUser = await userProvider.GetUserWithUserNameAsync(messageList.ContactUserUserName);

                if (contactUser == null)
                    return NotFound(Response<string>.GetError(null, $"There is no user with name: '{messageList.ContactUserUserName}'"));

                var data = await messageService.GetMessageInfoListAsync(messageList, senderUser, 
                                                                        contactUser);
                if (data != null)
                    return Ok(Response<List<MessageInfoResponse>>.GetSuccess(data));
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
            }
            return BadRequest(Response<string>.GetError(null, "An error occured"));
        }
    }
}