using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Threading.Tasks;
using MessagingServiceApp.Data.Entity;
using MessagingServiceApp.Dto.ApiParameter;
using MessagingServiceApp.Dto.ApiResponse;
using MessagingServiceApp.Interfaces;
using MessagingServiceApp.Libraries;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MessagingServiceApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> logger;
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        private readonly IAccountService accountService;

        public AccountController(ILogger<AccountController> logger,
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            IAccountService accountService)
        {
            this.logger = logger;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.accountService = accountService;
        }

        // POST api/account/register
        [HttpPost("register")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ServiceFilter(typeof(UserActivityLoggingActionFilter))]
        public async Task<IActionResult> Register([FromBody] RegisterParams model)
        {
            try
            {
                var existingUser = await userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                    return BadRequest(Response<string>.GetError(null, "User already exist"));

                var result = accountService.CreateUser(model);
                if (result.Succeeded)
                    return Created("", Response<RegisterParams>.GetSuccess(model));
                else
                    return new BadRequestObjectResult(Response<Dictionary<string, string>>
                        .GetError(null, "There is one or more register error", accountService.GetErrorObject(result.Errors)));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }

            return BadRequest(Response<string>.GetError(null, "An error occured"));
        }

        [HttpPost("login")]
        [Consumes(MediaTypeNames.Application.Json)]
        [Produces(MediaTypeNames.Application.Json)]
        [ServiceFilter(typeof(UserActivityLoggingActionFilter))]
        public async Task<IActionResult> Login([FromBody] LoginParams model)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user == null)
                    return NotFound(Response<string>.GetError(null, $"There is no user with email: '{model.Email}'"));

                var passwordCheck = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                if (!passwordCheck.Succeeded)
                    return BadRequest(Response<string>.GetError(null, $"User password is wrong with email '{model.Email}'"));

                var token = accountService.GetLoginToken(user);
                if (token != null)
                    return Ok(Response<object>.GetSuccess(new { token }));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
            return BadRequest(Response<string>.GetError(null, "An error occured"));
        }
    }
}