using System.Security.Claims;
using Affy.Core.Framework.GraphQl.Types;
using Affy.Core.Framework.Identity.Attributes;
using Affy.Core.Framework.Identity.Entities;
using Affy.Core.Framework.Identity.Extensions;
using Affy.Core.Framework.Identity.Rules;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Affy.Core.Framework.Identity.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class ChangeUserPasswordMutation
{
    [Guard]
    public async Task<IdentityResult> ChangeUserPassword (
        ChangeUserPasswordInput input,
        [Service] UserManager<User> userManager,
        ClaimsPrincipal claimsPrincipal
    )
    {
        var user = await userManager.FindByIdAsync(claimsPrincipal.GetId().ToString());

        return await userManager.ChangePasswordAsync(user, input.CurrentPassword, input.NewPassword);
    }

    public record ChangeUserPasswordInput(string CurrentPassword, string NewPassword);

    public class ChangeUserPasswordValidator : AbstractValidator<ChangeUserPasswordInput>
    {
        public ChangeUserPasswordValidator ()
        {
            RuleFor(_ => _.NewPassword)
                .StrongPassword();
        }
    }
}