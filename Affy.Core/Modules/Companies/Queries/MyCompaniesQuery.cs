using System.Security.Claims;
using Affy.Core.Framework.Database;
using Affy.Core.Framework.GraphQl.Types;
using Affy.Core.Framework.Identity.Attributes;
using Affy.Core.Framework.Identity.Extensions;
using Affy.Core.Modules.Companies.Entities;

namespace Affy.Core.Modules.Companies.Queries;

[ExtendObjectType(typeof(Query))]
public class MyCompaniesQuery
{
    [Guard]
    [UseOffsetPaging(MaxPageSize = 7, DefaultPageSize = 7)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Company> MyCompanies (MainDb db, ClaimsPrincipal claimsPrincipal)
    {
        return db.Set<Company>().Where(c => c.UserId == claimsPrincipal.GetId()).AsQueryable();
    }
}