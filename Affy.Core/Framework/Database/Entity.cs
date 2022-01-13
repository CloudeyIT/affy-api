using EntityFrameworkCore.Triggers;

namespace Affy.Core.Framework.Database;

public abstract class Entity
{
    static Entity ()
    {
        Triggers<Entity>.GlobalUpdating.Add(entry => { entry.Entity.Updated = DateTime.UtcNow; });

        Triggers<Entity>.GlobalUpdating.Add(entry => { entry.Entity.Revision = Ulid.NewUlid().ToGuid(); });

        Triggers<Entity>.GlobalInserting.Add(
            entry =>
            {
                var time = DateTime.UtcNow;
                entry.Entity.Created = time;
                entry.Entity.Updated = time;
            }
        );

        Triggers<Entity>.GlobalInserting.Add(entry => { entry.Entity.Revision = Ulid.NewUlid().ToGuid(); });
    }

    public Guid Id { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
    public Guid Revision { get; set; }
}