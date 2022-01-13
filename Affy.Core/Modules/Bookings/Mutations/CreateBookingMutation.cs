using Affy.Core.Framework.Database;
using Affy.Core.Framework.GraphQl.Types;
using Affy.Core.Modules.Bookings.Entities;
using Affy.Core.Modules.Companies.Entities;
using FluentValidation;
using HotChocolate.Execution;

namespace Affy.Core.Modules.Bookings.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class CreateBookingMutation
{
    public record CreateBookingInput
    (
        string GuestName,
        string? GuestEmail,
        string? GuestPhone,
        string CompanySlug,
        DateTime Start
    );

    public record CreateBookingPayload
    (
        DateTime Start,
        DateTime End,
        Guid BookingId
    );

    public class CreateBookingInputValidator : AbstractValidator<CreateBookingInput>
    {
        public CreateBookingInputValidator ()
        {
            RuleFor(_ => _.GuestName)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(_ => _.GuestEmail)
                .EmailAddress()
                .When(_ => !string.IsNullOrWhiteSpace(_.GuestEmail));

            RuleFor(_ => _.GuestPhone)
                .MaximumLength(20);
        }
    }

    public async Task<CreateBookingPayload> CreateBooking (CreateBookingInput input, MainDb db)
    {
        var company = await db.Set<Company>()
            .Include(
                c => c.OpeningTimes.Where(o => o.Start <= input.Start && o.End >= input.Start)
                    .OrderByDescending(o => o.Start)
            )
            .Where(c => c.Slug == input.CompanySlug.ToLower())
            .SingleOrDefaultAsync();

        if (company is null)
        {
            throw new QueryException(new Error("Company not found", "NO_COMPANY"));
        }

        var openingTime = company.OpeningTimes.FirstOrDefault();

        if (openingTime is null)
        {
            throw new QueryException(new Error("No opening time found", "NO_OPENING_TIME"));
        }

        var timeslot = openingTime.Timeslots.FirstOrDefault(t => t.Start <= input.Start && t.End > input.Start);
        
        if (timeslot is null)
        {
            throw new QueryException(new Error("No suitable timeslot", "NO_TIMESLOT"));
        }
        
        var bookingCount = await db.Set<Booking>()
            .Where(b => b.Start <= timeslot.Start && b.End >= timeslot.End)
            .CountAsync();

        if (bookingCount >= openingTime.TimeslotCapacity)
        {
            throw new QueryException(new Error("Timeslot is full", "TIMESLOT_FULL"));
        }

        var booking = new Booking
        {
            CompanyId = company.Id,
            Guest = new Guest
            {
                Name = input.GuestName.Trim(),
                Email = input.GuestEmail?.Trim(),
                Phone = input.GuestPhone?.Replace(" ", "").Replace("-", "").Trim(),
            },
            Start = timeslot.Start,
            End = timeslot.End,
        };

        db.Set<Booking>().Add(booking);

        await db.SaveChangesAsync();

        return new CreateBookingPayload(booking.Start, booking.End, booking.Id);
    }
}