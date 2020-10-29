using System.Collections.Generic;
using System.Threading.Tasks;
using MessagingServiceApp.Data.Entity;
using MessagingServiceApp.Dto.ApiParameter;
using MessagingServiceApp.Dto.ApiResponse;

namespace MessagingServiceApp.Interfaces
{
    public interface IMessageService
    {
        CreateMessageInfoResponse CreateMessageInfo(SendMessageParams message, 
                                                    User senderUser,
                                                    User contactUser);
        Task<List<MessageInfoResponse>> GetMessageInfoListAsync(MessageListParams messageList,
                                                                User senderUser,
                                                                User contactUser);
    }
}
