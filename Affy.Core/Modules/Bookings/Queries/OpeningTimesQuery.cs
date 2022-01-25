using Affy.Core.Framework.Database;
using Affy.Core.Framework.GraphQl.Types;
using Affy.Core.Modules.Bookings.Entities;

namespace Affy.Core.Modules.Bookings.Queries;

[ExtendObjectType(typeof(Query))]
public class OpeningTimesQuery
{
    public record OpeningTimesQueryInput(string CompanySlug);

    [UseOffsetPaging(MaxPageSize = 7, DefaultPageSize = 7)]
    [UseFiltering]
    [UseSorting]
    public IQueryable<OpeningTime> OpeningTimes (OpeningTimesQueryInput input, MainDb db)
    {
        return db.Set<OpeningTime>()
            .Include(o => o.Company)
            .Include(o => o.Bookings)
            .Where(o => o.Company!.Slug == input.CompanySlug)
            .AsQueryable();
    }
}