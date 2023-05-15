using Identity.Domain.Base;

namespace Identity.Domain;

public class OneTimePassword : Idable
{
    public string PhoneNumber { get; set; }
    public string Code { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime NotBefore { get; set; }
    public bool IsActive { get; set; }
}