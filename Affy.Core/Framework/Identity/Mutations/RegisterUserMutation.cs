using Affy.Core.Framework.Database;
using Affy.Core.Framework.Database.Rules;
using Affy.Core.Framework.GraphQl.Types;
using Affy.Core.Framework.Identity.Entities;
using Affy.Core.Framework.Identity.Rules;
using FairyBread;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Affy.Core.Framework.Identity.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class RegisterUserMutation
{
    public async Task<User> RegisterUser (RegisterUserInput input, [Service] UserManager<User> userManager)
    {
        var user = new User
        {
            Email = input.Email,
            UserName = input.Email,
            Name = input.Name,
        };

        var result = await userManager.CreateAsync(user, input.Password);

        if (!result.Succeeded)
        {
            throw new ApplicationException(
                $"Failed to register user: ({result.Errors.FirstOrDefault()?.Code}) {result.Errors.FirstOrDefault()?.Description}"
            );
        }

        await userManager.AddToRoleAsync(user, Role.User);

        return user;
    }

    /// <summary>
    ///     Details of the new user
    /// </summary>
    /// <param name="Email">E-mail address, must be shorter than 60 characters and unique</param>
    /// <param name="FirstName">First name of the user, must be between 1 and 60 characters</param>
    /// <param name="LastName">Last name of the user, must be between 1 and 60 characters</param>
    /// <param name="Password">
    ///     Password must be at least 10 characters long, and contain 1 lowercase, 1 uppercase, 1 special
    ///     character, and 1 digit
    /// </param>
    public record RegisterUserInput(
        string Email,
        string Name,
        string Password
    );

    public class RegisterUserValidator : AbstractValidator<RegisterUserInput>, IRequiresOwnScopeValidator
    {
        public RegisterUserValidator (MainDb db)
        {
            RuleFor(_ => _.Name)
                .NotEmpty()
                .MinimumLength(1)
                .MaximumLength(100);

            RuleFor(_ => _.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(100)
                .Unique(db, (User user) => user.NormalizedEmail, _ => _.ToUpper().Trim())
                .WithMessage("User with this email already exists");

            RuleFor(_ => _.Password)
                .StrongPassword();
        }
    }
}