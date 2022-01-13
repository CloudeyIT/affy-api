using Affy.Core.Framework.Identity.Attributes;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Affy.Core.Framework.Identity.Entities;

[Guard(Roles = new[] { Admin })]
public class Role : IdentityRole<Guid>
{
    public const string User = "User";
    public const string Admin = "Admin";

    public class Configuration : IEntityTypeConfiguration<Role>
    {
        public void Configure (EntityTypeBuilder<Role> builder)
        {
            builder.HasData(
                new Role
                {
                    Id = Guid.Parse("6278EB91-5E43-4761-B33A-7E7A8A0AF69C"),
                    ConcurrencyStamp = "B218D5C5-8FCB-4933-95C5-622FD4067087",
                    Name = "User",
                    NormalizedName = "USER",
                },
                new Role
                {
                    Id = Guid.Parse("FEB711FC-4E52-41E8-82C0-88E46D2044E4"),
                    ConcurrencyStamp = "BAEDCAEF-383A-41D2-ACF8-F45EEF2BF351",
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                }
            );
        }
    }
}