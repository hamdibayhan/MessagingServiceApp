using AutoMapper;
using MessagingServiceApp.Dto.ApiParameter;
using MessagingServiceApp.Dto.ApiResponse;

namespace MessagingServiceApp.Dto.Mapping
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<RegisterParams, RegisterResponse>();
        }
    }
}
