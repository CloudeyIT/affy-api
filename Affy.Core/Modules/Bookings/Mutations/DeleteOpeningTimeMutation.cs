using System.Security.Claims;
using Affy.Core.Framework.Database;
using Affy.Core.Framework.GraphQl.Types;
using Affy.Core.Framework.Identity.Attributes;
using Affy.Core.Framework.Identity.Extensions;
using Affy.Core.Modules.Bookings.Entities;
using HotChocolate.Execution;

namespace Affy.Core.Modules.Bookings.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class DeleteOpeningTimeMutation
{
    public record DeleteOpeningTimeInput(Guid Id);
    
    [Guard]
    public async Task DeleteOpeningTime(DeleteOpeningTimeInput input, MainDb db, ClaimsPrincipal claimsPrincipal)
    {
        var openingTime = await db.Set<OpeningTime>().Include(o => o.Company).SingleOrDefaultAsync(o => o.Id == input.Id);
        
        if (openingTime is null)
        {
            return;
        }

        if (openingTime.Company?.UserId != claimsPrincipal.GetId())
        {
            throw new QueryException(new Error("Not authorized", "NOT_AUTHORIZED"));
        }

        db.Remove(openingTime);
        await db.SaveChangesAsync();
    }
}