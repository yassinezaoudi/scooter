using AutoMapper;
using Gateway.Domain;
using Gateway.Dtos;
using Gateway.Endpoints.ScooterController.ViewModels;

namespace Gateway.Profiles;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        // Source -> Target
        CreateMap<User, UserInfoDto>()
            .ForMember(dest => dest.PhoneLastNumbers,
                opt => opt.MapFrom(
                    src => src.PhoneNumber.Substring(src.PhoneNumber.Length - 4)));
        CreateMap<UserRegisteredDto, User>()
            .ForMember(dest => dest.Id,
            opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.PhoneNumber,
            opt => opt.MapFrom(src => src.PhoneNumber));

        CreateMap<PasswordGeneratorDto, RequestPasswordDto>()
            .ForMember(dest => dest.PhoneNumber,
                opt => opt.MapFrom(
                    src => src.PhoneNumber))
            .ForMember(dest => dest.Event,
                opt => opt.MapFrom(
                    src => "Password_Requested"));

        CreateMap<AddScooterViewModel, RequestAddNewScooterDto>()
            .ForMember(dest => dest.Model,
                opt => opt.MapFrom(
                    src => src.Model))
            .ForMember(dest => dest.LinkToScooter,
                opt => opt.MapFrom(
                    src => src.LinkToScooter));
    }

}