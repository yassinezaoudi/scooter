namespace Identity.Dtos;

public class PublishUserDto : GenericEventDto
{
    public Guid Id { get; set; }
    public string PhoneNumber { get; set; }
}