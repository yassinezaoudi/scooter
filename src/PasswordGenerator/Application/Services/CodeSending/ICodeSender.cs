namespace Password_Generator.Application.Services.CodeSending;

public interface ICodeSender
{
    Task<string?> SendCode(string phoneNumber); 
}