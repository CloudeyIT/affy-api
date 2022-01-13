using Affy.Core.Framework.Database;
using Affy.Core.Framework.Identity.Entities;
using Affy.Core.Modules.Bookings.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Affy.Core.Modules.Companies.Entities;

public class Company : Entity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    public bool Approved { get; set; }

    [GraphQLIgnore]
    public User User { get; set; } = default!;

    [GraphQLIgnore]
    public Guid UserId { get; set; }

    public List<OpeningTime> OpeningTimes { get; set; } = new();

    public class Configuration : EntityConfiguration<Company>
    {
        public override void Configure (EntityTypeBuilder<Company> builder)
        {
            base.Configure(builder);

            builder.HasIndex(_ => _.Slug).IsUnique();
        }
    }
}