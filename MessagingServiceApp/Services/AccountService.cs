using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MessagingServiceApp.Data.Entity;
using MessagingServiceApp.Dto.ApiParameter;
using MessagingServiceApp.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace MessagingServiceApp.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> userManager;
        private readonly IConfiguration config;

        public AccountService(UserManager<User> userManager,
            IConfiguration config)
        {
            this.userManager = userManager;
            this.config = config;
        }

        public IdentityResult CreateUser(RegisterParams model)
        {
            User user = new User
            {
                UserName = model.UserName,
                Email = model.Email
            };

            return userManager.CreateAsync(user, model.Password).Result;
        }

        public string GetLoginToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email)
                };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtToken:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                // Todo - Move expires hour to appsettings file
                Expires = DateTime.UtcNow.AddHours(3),
                SigningCredentials = credentials
            };

            var tokenObject = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenObject != null ? tokenHandler.WriteToken(tokenObject) : null;

            return token;
        }

        public Dictionary<string, string> GetErrorObject(IEnumerable<IdentityError> errors)
        {
            var resultErrors = new Dictionary<string, string>();
            foreach (var error in errors)
            {
                resultErrors.Add(error.Code, error.Description);
            }

            return resultErrors;
        }
    }
}
