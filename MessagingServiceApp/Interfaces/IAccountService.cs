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
        string GetLoginToken(User user);
        Dictionary<string, string> GetErrorObject(IEnumerable<IdentityError> errors);
    }
}
