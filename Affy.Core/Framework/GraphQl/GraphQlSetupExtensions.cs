using System.Reflection;
using Affy.Core.Framework.Database;
using Affy.Core.Framework.GraphQl.Types;
using HotChocolate.Execution.Configuration;
using HotChocolate.Types.Pagination;

namespace Affy.Core.Framework.GraphQl;

public static class GraphQlSetupExtensions
{
    public static IRequestExecutorBuilder AddGraphQl (
        this IServiceCollection services,
        IEnumerable<Assembly>? assemblies = default
    )
    {
        var builder = services.AddGraphQLServer();

        builder.UseQueries();
        builder.UseMutations();

        // Uncomment to enable subscriptions
        // builder.UseSubscriptions();

        builder
            .AddTypeExtensionsFromAssemblies(assemblies ?? AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies())
            .AddFairyBread(_ => _.ThrowIfNoValidatorsFound = false)
            .AddAuthorization()
            .AddProjections()
            .AddFiltering()
            .AddSorting()
            .SetPagingOptions(
                new PagingOptions
                {
                    IncludeTotalCount = true,
                    MaxPageSize = 50,
                    DefaultPageSize = 50,
                }
            )
            .AddErrorFilter<LoggingErrorFilter>()
            .UseAutomaticPersistedQueryPipeline()
            .AddInMemoryQueryStorage()
            .AddDefaultTransactionScopeHandler()
            .RegisterDbContext<MainDb>(DbContextKind.Pooled)
            .AddMutationConventions(
                new MutationConventionOptions
                {
                    ApplyToAllMutations = false,
                    PayloadTypeNamePattern = "{MutationName}Result",
                }
            )
            .InitializeOnStartup();

        return builder;
    }

    public static IRequestExecutorBuilder AddTypeExtensionsFromAssemblies (
        this IRequestExecutorBuilder builder,
        IEnumerable<Assembly> assemblies
    )
    {
        assemblies.ForEach(
            assembly =>
            {
                assembly.GetTypes()
                    .Where(type => type.GetCustomAttribute<ExtendObjectTypeAttribute>() is not null)
                    .ForEach(type => builder.AddTypeExtension(type));
            }
        );


        return builder;
    }

    public static IRequestExecutorBuilder UseQueries (this IRequestExecutorBuilder builder)
    {
        return builder.AddQueryType<Query>();
    }

    public static IRequestExecutorBuilder UseMutations (this IRequestExecutorBuilder builder)
    {
        return builder.AddMutationType<Mutation>();
    }

    public static IRequestExecutorBuilder UseSubscriptions (this IRequestExecutorBuilder builder)
    {
        return builder.AddSubscriptionType<Subscription>().AddInMemorySubscriptions();
    }
}