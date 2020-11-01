using System.Collections.Generic;
using MessagingServiceApp.Data.Entity;
using MessagingServiceApp.Dto.ApiParameter;
using MessagingServiceApp.Dto.ApiResponse;
using Microsoft.AspNetCore.Identity;

namespace MessagingServiceApp.Interfaces
{
    public interface IAccountService
    {
        IdentityResult CreateUser(RegisterParams model);
        string GetLoginToken(User user);
        Dictionary<string, string> GetIdentityErrorObject(IEnumerable<IdentityError> errors);
        RegisterResponse GetRegisterResponseObject(RegisterParams model);
    }
}
