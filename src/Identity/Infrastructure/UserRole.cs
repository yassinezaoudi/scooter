using Identity.Domain.Base;

namespace Identity.Infrastructure;

public class UserRole : Idable
{
    public User User { get; set; }
    public Role Role { get; set; }
}