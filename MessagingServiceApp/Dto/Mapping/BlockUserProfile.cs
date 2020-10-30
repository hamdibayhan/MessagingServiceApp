using AutoMapper;
using MessagingServiceApp.Data.MongoModels;
using MessagingServiceApp.Dto.ApiResponse;

namespace MessagingServiceApp.Dto.Mapping
{
    public class BlockUserProfile : Profile
    {
        public BlockUserProfile()
        {
            CreateMap<BlockUserInfo, BlockUserResponse>();
        }
    }
}
