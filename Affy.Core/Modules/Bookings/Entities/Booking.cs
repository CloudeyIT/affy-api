using Affy.Core.Framework.Database;
using Affy.Core.Modules.Companies.Entities;

namespace Affy.Core.Modules.Bookings.Entities;

public class Booking : Entity
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    
    public Company? Company { get; set; }
    public Guid CompanyId { get; set; }

    public Guest? Guest { get; set; }
    public Guid GuestId { get; set; }

    public class Configuration : EntityConfiguration<Booking> { }
}