using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using MessagingServiceApp.Data.Entity;
using MessagingServiceApp.Dto.ApiParameter;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace MessagingServiceApp.Interfaces
{
    public interface IAccountService
    {
        IdentityResult CreateUser(RegisterParams model);
        SecurityToken GetLoginToken(User user, JwtSecurityTokenHandler tokenHandler);
        Dictionary<string, string> GetErrorObject(IEnumerable<IdentityError> errors);
    }
}
