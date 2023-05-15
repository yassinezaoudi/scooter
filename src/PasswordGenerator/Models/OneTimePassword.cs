namespace Password_Generator.Models;

public class OneTimePassword
{
    public Guid Id { get; set; }
    public string PhoneNumber { get; set; }
    public string Code { get; set; }
    public DateTime NotBefore { get; set; }
    public DateTime ExpiresAt { get; set; }

    public OneTimePassword(string phoneNumber, Random random, int length = 4) : this(phoneNumber)
    {
        const string chars = "0123456789";
        Code = new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public OneTimePassword(string phoneNumber, string code) : this(phoneNumber)
    {
        Code = code;
    }

    public OneTimePassword(string phoneNumber)
    {
        PhoneNumber = phoneNumber;
        NotBefore = DateTime.UtcNow;
        ExpiresAt = DateTime.UtcNow.AddMinutes(int.Parse(
            Environment.GetEnvironmentVariable("MinutesToExpirePassword")!));
    }
}