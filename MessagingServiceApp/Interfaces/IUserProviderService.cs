using System.Threading.Tasks;
using MessagingServiceApp.Data.Entity;

namespace MessagingServiceApp.Interfaces
{
    public interface IUserProviderService
    {
        Task<User> GetCurrentUser();
        Task<User> GetUserWithUserNameAsync(string userName);
        Task<User> GetUserWithEmailAsync(string userEmail);
    }
}
