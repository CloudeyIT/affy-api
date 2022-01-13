using Affy.Core.Framework.Database;

namespace Affy.Core.Modules.Bookings.Entities;

public class Guest : Entity
{
    public string Name { get; set; } = default!;
    public string? Email { get; set; }
    public string? Phone { get; set; }

    public class Configuration : EntityConfiguration<Guest> { }
}