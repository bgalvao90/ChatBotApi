using AutoMapper;

namespace ChatBotApi.DTOs.Mapping
{
    public class LoginDTOMappingProfile : Profile
    {
        public LoginDTOMappingProfile()
        {
            CreateMap<UserModel, LoginDto>().ReverseMap();
            CreateMap<UserModel, RegisterUserDto>().ReverseMap();
            CreateMap<UserModel, RegisterAtendenteDto>().ReverseMap();
            CreateMap<UserModel, RegisterClienteDto>().ReverseMap();
        }
    }
}
