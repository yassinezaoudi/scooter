using AutoMapper;
using FreeScooters.Dtos;
using FreeScooters.Models;

namespace FreeScooters.Profiles;

public class FreeScooterProfile : Profile
{
    public FreeScooterProfile()
    {
        // Source -> Target
        CreateMap<Scooter, FreeScooterInfoDto>();
    }
}