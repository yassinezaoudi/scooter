namespace Order.Application.Services.EmailSender;

public interface IEmailSender
{
    Task<IResult> SendEmail(string message, Guid userId);
}