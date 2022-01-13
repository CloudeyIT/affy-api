using Autofac;
using Microsoft.AspNetCore.Authorization;

namespace Affy.Core.Framework.Identity.Providers;

public class AuthorizationHandlerProvider : IAuthorizationHandlerProvider
{
    private readonly ILifetimeScope _scope;

    public AuthorizationHandlerProvider (ILifetimeScope scope)
    {
        _scope = scope;
    }

    public Task<IEnumerable<IAuthorizationHandler>> GetHandlersAsync (AuthorizationHandlerContext context)
    {
        var handlers = new List<IAuthorizationHandler>();

        foreach (var requirement in context.Requirements)
            if (requirement.GetType()
                    .IsAssignableTo(typeof(AuthorizationHandler<>).MakeGenericType(requirement.GetType())) ||
                context.Resource is not null &&
                requirement.GetType()
                    .IsAssignableTo(
                        typeof(AuthorizationHandler<,>).MakeGenericType(
                            requirement.GetType(),
                            context.Resource.GetType()
                        )
                    ))
            {
                handlers.Add((requirement as IAuthorizationHandler)!);
            }
            else
            {
                handlers.AddRange(ResolveGeneralHandlers(requirement));
                handlers.AddRange(ResolveResourceHandlers(requirement, context));
            }

        return Task.FromResult(handlers.AsEnumerable());
    }

    private IEnumerable<IAuthorizationHandler> ResolveGeneralHandlers (IAuthorizationRequirement requirement)
    {
        return _scope.ResolveOptional(
                   typeof(IEnumerable<>).MakeGenericType(
                       typeof(AuthorizationHandler<>).MakeGenericType(requirement.GetType())
                   ),
                   new TypedParameter(requirement.GetType(), requirement)
               ) as IEnumerable<IAuthorizationHandler> ??
               new List<IAuthorizationHandler>();
    }

    private IEnumerable<IAuthorizationHandler> ResolveResourceHandlers (
        IAuthorizationRequirement requirement,
        AuthorizationHandlerContext context
    )
    {
        if (context.Resource is null) return new List<IAuthorizationHandler>();

        return _scope.ResolveOptional(
                   typeof(IEnumerable<>).MakeGenericType(
                       typeof(AuthorizationHandler<,>).MakeGenericType(
                           requirement.GetType(),
                           context.Resource!.GetType()
                       )
                   ),
                   new TypedParameter(requirement.GetType(), requirement)
               ) as IEnumerable<IAuthorizationHandler> ??
               new List<IAuthorizationHandler>();
    }
}