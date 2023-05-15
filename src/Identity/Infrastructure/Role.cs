using Identity.Domain.Base;

namespace Identity.Infrastructure;

public class Role : Idable
{
    public string Name { get; set; }
    public string NormalizedName { get; set; }
}