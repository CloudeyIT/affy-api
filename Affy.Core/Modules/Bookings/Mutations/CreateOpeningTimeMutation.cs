using System.Security.Claims;
using Affy.Core.Framework.Database;
using Affy.Core.Framework.GraphQl.Types;
using Affy.Core.Framework.Identity.Attributes;
using Affy.Core.Framework.Identity.Extensions;
using Affy.Core.Modules.Bookings.Entities;
using Affy.Core.Modules.Companies.Entities;
using FluentValidation;
using HotChocolate.Execution;

namespace Affy.Core.Modules.Bookings.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class CreateOpeningTimeMutation
{
    public record CreateOpeningTimeInput(
        Guid CompanyId,
        DateTime Start,
        DateTime End,
        int TimeslotInterval,
        int TimeslotCapacity
    );

    public record CreateOpeningTimePayload(
        Guid Id,
        DateTime Start,
        DateTime End,
        int TimeslotInterval,
        int TimeslotCapacity
    );

    public class CreateOpeningTimeValidator : AbstractValidator<CreateOpeningTimeInput>
    {
        public CreateOpeningTimeValidator ()
        {
            RuleFor(_ => _.CompanyId).NotEmpty();
            RuleFor(_ => _.Start).NotEmpty();
            RuleFor(_ => _.End).NotEmpty().GreaterThan(_ => _.Start);
            RuleFor(_ => _.TimeslotInterval).NotEmpty().GreaterThan(0).LessThanOrEqualTo(360);
            RuleFor(_ => _.TimeslotCapacity).NotEmpty().GreaterThan(0).LessThanOrEqualTo(10000);
        }
    }

    [Guard]
    public async Task<CreateOpeningTimePayload> CreateOpeningTime (
        CreateOpeningTimeInput input,
        MainDb db,
        ClaimsPrincipal claimsPrincipal
    )
    {
        var company = await db.Set<Company>()
            .Include(
                c => c.OpeningTimes
                    .Where(
                        o =>
                            (o.Start <= input.Start && o.End > input.Start) ||
                            (o.Start >= input.Start && o.Start < input.End)
                    )
            )
            .Where(c => c.Id == input.CompanyId)
            .SingleOrDefaultAsync();

        if (company is null)
        {
            throw new QueryException(new Error("Company not found", "NOT_FOUND"));
        }

        if (company.UserId != claimsPrincipal.GetId())
        {
            throw new QueryException(new Error("Not allowed to access company", "NOT_AUTHORIZED"));
        }

        if (company.OpeningTimes.Any())
        {
            throw new QueryException(new Error("Overlaps with existing opening time", "OVERLAP"));
        }

        var openingTime = new OpeningTime
        {
            CompanyId = input.CompanyId,
            Start = input.Start,
            End = input.End,
            TimeslotInterval = input.TimeslotInterval,
            TimeslotCapacity = input.TimeslotCapacity
        };

        db.Set<OpeningTime>().Add(openingTime);
        await db.SaveChangesAsync();

        return new CreateOpeningTimePayload(
            openingTime.Id,
            openingTime.Start,
            openingTime.End,
            openingTime.TimeslotInterval,
            openingTime.TimeslotCapacity
        );
    }
}