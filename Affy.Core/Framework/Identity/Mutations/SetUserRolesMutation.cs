using Affy.Core.Framework.Database;
using Affy.Core.Framework.Database.Rules;
using Affy.Core.Framework.GraphQl.Types;
using Affy.Core.Framework.Identity.Attributes;
using Affy.Core.Framework.Identity.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Affy.Core.Framework.Identity.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class SetUserRolesMutation
{
    [Guard(Roles = new[] { Role.Admin })]
    public async Task<IList<string>> SetUserRoles (
        SetUserRolesInput input,
        [Service] UserManager<User> userManager
    )
    {
        var user = await userManager.FindByIdAsync(input.Id.ToString());
        var currentRoles = await userManager.GetRolesAsync(user);

        var rolesToAdd = input.Roles.Except(currentRoles);
        var rolesToRemove = currentRoles.Except(input.Roles).Except(new[] { Role.Admin });

        await userManager.RemoveFromRolesAsync(user, rolesToRemove);
        await userManager.AddToRolesAsync(user, rolesToAdd);

        return await userManager.GetRolesAsync(user);
    }

    public record SetUserRolesInput(Guid Id, string[] Roles);

    public class SetUserRolesValidator : AbstractValidator<SetUserRolesInput>
    {
        public SetUserRolesValidator (MainDb db)
        {
            RuleFor(_ => _.Id)
                .Exists(db, (User _) => _.Id);

            RuleFor(_ => _.Roles)
                .ForEach(role => role.Exists(db, (Role _) => _.Name));
        }
    }
}