using Affy.Core.Framework.Database;
using Affy.Core.Framework.GraphQl.Types;
using Affy.Core.Framework.Identity.Attributes;
using Affy.Core.Framework.Identity.Entities;

namespace Affy.Core.Framework.Identity.Queries;

[ExtendObjectType(typeof(Query))]
public class UsersQuery
{
    [Guard(Roles = new[] { Role.Admin })]
    [UseOffsetPaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<User> Users (MainDb db)
    {
        return db.Set<User>().AsNoTracking().AsQueryable();
    }
}