using Affy.Core.Framework.Database;
using Affy.Core.Framework.Identity.Attributes;
using Affy.Core.Modules.Companies.Entities;

namespace Affy.Core.Modules.Bookings.Entities;

public class Booking : Entity
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    
    public Company? Company { get; set; }
    public Guid CompanyId { get; set; }

    [Guard]
    public Guest? Guest { get; set; }
    [Guard]
    public Guid GuestId { get; set; }
    
    public OpeningTime? OpeningTime { get; set; }
    public Guid? OpeningTimeId { get; set; } = null;

    public class Configuration : EntityConfiguration<Booking> { }
}