using Autofac;
using Affy.Core.Framework.Setup;
using EntityFrameworkCore.Triggers;

namespace Affy.Core.Framework.Database;

public class DatabaseModule : Module
{
    protected override void Load (ContainerBuilder builder)
    {
        builder.RegisterGeneric(typeof(Triggers<,>))
            .As(typeof(ITriggers<,>))
            .SingleInstance();

        builder.RegisterGeneric(typeof(Triggers<>))
            .As(typeof(ITriggers<>))
            .SingleInstance();

        builder.RegisterType(typeof(Triggers))
            .As(typeof(ITriggers))
            .SingleInstance();

        AppDomain.CurrentDomain.GetIncludedAssemblies()
            .ForEach(
                assembly => builder.RegisterAssemblyTypes(assembly)
                    .AssignableTo<Entity>()
                    .AsSelf()
            );

        builder.Register(
                context => context.Resolve<IDbContextFactory<MainDb>>().CreateDbContext()
            )
            .AsSelf()
            .AsImplementedInterfaces();
    }
}