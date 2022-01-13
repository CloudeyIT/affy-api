using System.Security.Claims;
using Affy.Core.Framework.Database;
using Affy.Core.Framework.GraphQl.Types;
using Affy.Core.Framework.Identity.Extensions;
using Affy.Core.Modules.Bookings.Entities;
using HotChocolate.Execution;

namespace Affy.Core.Modules.Bookings.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class DeleteBookingMutation
{
    public record DeleteBookingInput(Guid Id);

    public async Task DeleteBooking (DeleteBookingInput input, MainDb db, ClaimsPrincipal claimsPrincipal)
    {
        var booking = await db.Set<Booking>().Include(o => o.Company).SingleOrDefaultAsync(o => o.Id == input.Id);
        
        if (booking is null)
        {
            return;
        }

        if (booking.Company?.UserId != claimsPrincipal.GetId())
        {
            throw new QueryException(new Error("Not authorized", "NOT_AUTHORIZED"));
        }

        db.Remove(booking);
        await db.SaveChangesAsync();
    }
}