using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;

namespace Affy.Core.Framework.Validation;

public static class ValidationServiceRegistrationExtensions
{
    public static IServiceCollection AddValidation (this IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        services.AddFluentValidation();
        services.AddValidatorsFromAssemblies(assemblies);

        return services;
    }
}