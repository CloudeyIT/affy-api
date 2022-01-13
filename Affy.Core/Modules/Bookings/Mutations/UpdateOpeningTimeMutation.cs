using System.Security.Claims;
using Affy.Core.Framework.Database;
using Affy.Core.Framework.GraphQl.Types;
using Affy.Core.Framework.Identity.Attributes;
using Affy.Core.Framework.Identity.Extensions;
using Affy.Core.Modules.Bookings.Entities;
using FluentValidation;
using HotChocolate.Execution;

namespace Affy.Core.Modules.Bookings.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class UpdateOpeningTimeMutation
{
    public record UpdateOpeningTimeInput (
        Guid Id,
        DateTime Start,
        DateTime End,
        int TimeslotInterval,
        int TimeslotCapacity
    );
    
    public record UpdateOpeningTimePayload(
        Guid Id,
        DateTime Start,
        DateTime End,
        int TimeslotInterval,
        int TimeslotCapacity
    );
    
    public class UpdateOpeningTimeValidator : AbstractValidator<UpdateOpeningTimeInput>
    {
        public UpdateOpeningTimeValidator ()
        {
            RuleFor(_ => _.Id).NotEmpty();
            RuleFor(_ => _.Start).NotEmpty();
            RuleFor(_ => _.End).NotEmpty().GreaterThan(_ => _.Start);
            RuleFor(_ => _.TimeslotInterval).NotEmpty().GreaterThan(0).LessThanOrEqualTo(360);
            RuleFor(_ => _.TimeslotCapacity).NotEmpty().GreaterThan(0).LessThanOrEqualTo(10000);
        }
    }

    [Guard]
    public async Task<UpdateOpeningTimePayload> UpdateOpeningTime (
        UpdateOpeningTimeInput input,
        MainDb db,
        ClaimsPrincipal claimsPrincipal
    )
    {
        var openingTime =
            await db.Set<OpeningTime>().Include(o => o.Company).SingleOrDefaultAsync(o => o.Id == input.Id);

        if (openingTime?.Company?.UserId != claimsPrincipal.GetId())
        {
            throw new QueryException(new Error("Opening time not found", "NOT_FOUND"));
        }

        input.Adapt(openingTime);

        await db.SaveChangesAsync();

        return openingTime!.Adapt<UpdateOpeningTimePayload>();
    }
}