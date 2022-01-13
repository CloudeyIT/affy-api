using Affy.Core.Framework.Database;
using Affy.Core.Framework.GraphQl.Types;
using Affy.Core.Framework.Identity.Attributes;
using Affy.Core.Framework.Identity.Entities;

namespace Affy.Core.Framework.Identity.Queries;

[ExtendObjectType(typeof(Query))]
public class RolesQuery
{
    [Guard(Roles = new[] { Role.User })]
    [UseOffsetPaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Role> Roles (MainDb db)
    {
        return db.Set<Role>().AsNoTracking().AsQueryable();
    }
}