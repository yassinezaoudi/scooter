using AutoMapper;
using Scooter_Controller.Dtos;
using Scooter_Controller.Models;

namespace Scooter_Controller.Profiles;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        // Source -> Target
        CreateMap<AddNewScooterDto, Scooter>()
            .ForMember(dest => dest.Model,
                opt => 
                    opt.MapFrom(src => src.Model))
            .ForMember(dest => dest.LinkToScooter,
                opt => 
                    opt.MapFrom(src => src.LinkToScooter))
            .ForMember(dest => dest.IsAvailableToRent,
                opt => 
                    opt.MapFrom(src => false));
        
    }
}