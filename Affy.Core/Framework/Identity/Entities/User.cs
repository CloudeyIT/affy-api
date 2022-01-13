using Microsoft.AspNetCore.Identity;

namespace Affy.Core.Framework.Identity.Entities;

public class User : IdentityUser<Guid>
{
    [IsProjected(true)]
    public override Guid Id { get; set; }

    [IsProjected(true)]
    public string Name { get; set; } = default!;
    
    [GraphQLIgnore]
    public override string? PasswordHash { get; set; }

    [GraphQLIgnore]
    public override string? SecurityStamp { get; set; }
}