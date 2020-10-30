using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MessagingServiceApp.Data.Entity;
using MessagingServiceApp.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace MessagingServiceApp.Services
{
    public class UserProviderService : IUserProviderService
    {
        private readonly IHttpContextAccessor context;
        private readonly UserManager<User> userManager;

        public UserProviderService(IHttpContextAccessor context, UserManager<User> userManager)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.userManager = userManager;
        }

        public async Task<User> GetCurrentUser()
        {
            var currentUserObjectEmail = context.HttpContext.User.Claims
                       .First(i => i.Type == ClaimTypes.Email).Value;

            return await GetUserWithEmailAsync(currentUserObjectEmail);
        }

        public async Task<User> GetUserWithUserNameAsync(string userName)
        {
            return await userManager.FindByNameAsync(userName);
        }

        public async Task<User> GetUserWithEmailAsync(string userEmail)
        {
            return await userManager.FindByEmailAsync(userEmail);
        }
    }
}
