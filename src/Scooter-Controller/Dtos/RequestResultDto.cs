using System.Globalization;
using Scooter_Controller.Models;

namespace Scooter_Controller.Dtos;

public class RequestResultDto
{
    public bool IsSuccess { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }

    public RequestResultDto(bool isSuccess, SpaceTime spaceTime)
    {
        this.IsSuccess = isSuccess;
        this.Latitude = spaceTime.Latitude;
        this.Longitude = spaceTime.Longtitude;
    }

    public RequestResultDto()
    {
        IsSuccess = false;
        Latitude = 0;
        Longitude = 0;
    }
}