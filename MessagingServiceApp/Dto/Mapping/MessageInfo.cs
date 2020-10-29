using AutoMapper;
using MessagingServiceApp.Data.MongoModel;
using MessagingServiceApp.Dto.ApiResponse;

namespace MessagingServiceApp.Dto.Mapping
{
    public class MessageInfoProfile : Profile
    {
        public MessageInfoProfile()
        {
            CreateMap<MessageInfo, CreateMessageInfoResponse>();
            CreateMap<MessageInfo, MessageInfoResponse>();
        }
    }
}
