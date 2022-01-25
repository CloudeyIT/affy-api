using Affy.Core.Framework.Database;
using Affy.Core.Framework.Identity.Attributes;
using Affy.Core.Modules.Companies.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Affy.Core.Modules.Bookings.Entities;

public class OpeningTime : Entity
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    [Guard]
    public Company? Company { get; set; }

    public Guid CompanyId { get; set; }

    public int TimeslotInterval { get; set; } = 15;
    public int TimeslotCapacity { get; set; } = 10;

    [GraphQLIgnore]
    public List<Booking> Bookings { get; set; } = new();

    public IEnumerable<Timeslot> Timeslots => Enumerable
        .Range(1, Convert.ToInt32(Math.Floor((End - Start).TotalMinutes / TimeslotInterval)))
        .Select(
            i => new Timeslot(
                Start.AddMinutes((i - 1) * TimeslotInterval),
                Start.AddMinutes(i * TimeslotInterval),
                Bookings.Count(b => b.Start >= Start.AddMinutes((i - 1) * TimeslotInterval) && b.End <= Start.AddMinutes(i * TimeslotInterval))
            )
        );

    public record Timeslot(DateTime Start, DateTime End, int? BookingsCount = null);

    public class Configuration : EntityConfiguration<OpeningTime>
    {
        public override void Configure (EntityTypeBuilder<OpeningTime> builder)
        {
            base.Configure(builder);

            builder.HasOne(x => x.Company)
                .WithMany(x => x.OpeningTimes)
                .HasForeignKey(x => x.CompanyId);

            builder.Ignore(x => x.Timeslots);

            builder.HasMany(x => x.Bookings)
                .WithOne(x => x.OpeningTime);
        }
    }
}