using Autofac;
using Affy.Core.Framework.Database;
using Affy.Core.Framework.Identity;
using Affy.Core.Framework.Setup;
using Affy.Core.Framework.Validation;

[assembly: IncludeAssembly]

namespace Affy.Core;

public class CoreModule : Module
{
    protected override void Load (ContainerBuilder builder)
    {
        // Do not map null values from source to destination when using .Adapt() extension method. This is useful for
        // edit scenarios, but must be disabled in certain circumstances where we do want to overwrite with nulls.
        TypeAdapterConfig.GlobalSettings.Default.IgnoreNullValues(true);

        builder.RegisterModule<DatabaseModule>();
        builder.RegisterModule<IdentityModule>();
        builder.RegisterModule<ValidationModule>();
    }
}