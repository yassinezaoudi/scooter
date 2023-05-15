namespace Gateway.Dtos;

public class RequestPasswordDto : GenericEventDto
{
    public string PhoneNumber { get; set; }
    public string Ip { get; set; }
}