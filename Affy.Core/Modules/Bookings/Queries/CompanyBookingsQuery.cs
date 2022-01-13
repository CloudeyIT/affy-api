using System.Security.Claims;
using Affy.Core.Framework.Database;
using Affy.Core.Framework.GraphQl.Types;
using Affy.Core.Framework.Identity.Attributes;
using Affy.Core.Framework.Identity.Extensions;
using Affy.Core.Modules.Bookings.Entities;
using Affy.Core.Modules.Companies.Entities;
using HotChocolate.Execution;

namespace Affy.Core.Modules.Bookings.Queries;

[ExtendObjectType(typeof(Query))]
public class CompanyBookingsQuery
{
    public record CompanyBookingsQueryInput(Guid CompanyId);
    
    [Guard]
    [UseOffsetPaging(MaxPageSize = 7, DefaultPageSize = 7)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<Booking>> CompanyBookings (CompanyBookingsQueryInput input, MainDb db, ClaimsPrincipal claimsPrincipal)
    {
        var company = await db.FindAsync<Company>(input.CompanyId);

        if (company?.UserId != claimsPrincipal.GetId())
        {
            throw new QueryException(new Error("Company not found", "COMPANY_NOT_FOUND"));
        }
        
        return db.Set<Booking>().Include(b => b.Guest).Where(b => b.CompanyId == input.CompanyId);
    }
}