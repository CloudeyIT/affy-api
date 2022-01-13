using Affy.Core.Framework.Database.ValueGenerators;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Affy.Core.Framework.Database;

public abstract class EntityConfiguration<T> : IEntityTypeConfiguration<T> where T : Entity
{
    public virtual void Configure (EntityTypeBuilder<T> builder)
    {
        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasValueGenerator<GuidValueGenerator>();
    }
}