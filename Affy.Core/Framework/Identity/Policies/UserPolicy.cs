using Affy.Core.Framework.Identity.Entities;
using Affy.Core.Framework.Identity.Extensions;
using Affy.Core.Framework.Identity.Types;
using HotChocolate.Resolvers;
using Microsoft.AspNetCore.Authorization;

namespace Affy.Core.Framework.Identity.Policies;

public class UserPolicy : IPolicy
{
    public AuthorizationPolicy? Policy { get; } = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole(Role.User, Role.Admin)
        .AddRequirements(new CanAccessUserRequirement())
        .Build();
}

public record CanAccessUserRequirement : IAuthorizationRequirement;

public class CanAccessUserRequirementHandler : AuthorizationHandler<CanAccessUserRequirement>
{
    protected override Task HandleRequirementAsync (
        AuthorizationHandlerContext context,
        CanAccessUserRequirement requirement
    )
    {
        if (context.Resource is not IDirectiveContext { Result: User user })
        {
            return Task.CompletedTask;
        }

        // User can access their own User
        if (context.User.GetId() == user.Id)
        {
            context.Succeed(requirement);
        }

        // Admins can access all users
        if (context.User.IsInRole(Role.Admin))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}