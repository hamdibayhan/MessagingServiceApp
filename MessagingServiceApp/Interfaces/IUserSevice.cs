using MessagingServiceApp.Data.Entity;
using MessagingServiceApp.Dto.ApiResponse;

namespace MessagingServiceApp.Interfaces
{
    public interface IUserService
    {
        BlockUserResponse BlockUser(User blockingUser, User blockedUser);
        bool IsUserAlreadyBlocked(User blockingUser, User blockedUser);
    }
}
