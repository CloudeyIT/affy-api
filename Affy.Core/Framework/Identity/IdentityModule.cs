using Autofac;
using Affy.Core.Framework.Identity.Providers;
using Affy.Core.Framework.Identity.Services;
using Affy.Core.Framework.Identity.Types;
using Affy.Core.Framework.Setup;
using Microsoft.AspNetCore.Authorization;

namespace Affy.Core.Framework.Identity;

public class IdentityModule : Module
{
    public const string Name = "Identity";

    protected override void Load (ContainerBuilder builder)
    {
        var assemblies = AppDomain.CurrentDomain.GetIncludedAssemblies();

        builder.RegisterType<AuthorizationPolicyProvider>()
            .As<IAuthorizationPolicyProvider>()
            .SingleInstance();

        builder.RegisterType<AuthorizationHandlerProvider>()
            .As<IAuthorizationHandlerProvider>()
            .SingleInstance();

        builder.RegisterType<AuthorizationService>()
            .As<IAuthorizationService>()
            .SingleInstance();

        builder.RegisterType<TokenService>()
            .AsSelf()
            .SingleInstance();

        assemblies.ForEach(
            assembly =>
            {
                builder.RegisterAssemblyTypes(assembly)
                    .AsClosedTypesOf(typeof(AuthorizationHandler<>))
                    .As<IAuthorizationHandler>()
                    .InstancePerLifetimeScope();

                builder.RegisterAssemblyTypes(assembly)
                    .AsClosedTypesOf(typeof(AuthorizationHandler<,>))
                    .As<IAuthorizationHandler>()
                    .InstancePerLifetimeScope();

                assembly.GetTypes()
                    .Where(type => type.IsAssignableTo<IPolicy>() && !type.IsInterface)
                    .ForEach(
                        policy =>
                        {
                            var policyInstance = (IPolicy?)Activator.CreateInstance(policy);

                            var authorizationPolicy = policyInstance?.Policy;

                            if (authorizationPolicy is null) return;

                            builder.RegisterInstance(authorizationPolicy)
                                .Named<AuthorizationPolicy>(policy.Name)
                                .As<AuthorizationPolicy>()
                                .SingleInstance();
                        }
                    );
            }
        );
    }
}