using System.Reflection;

namespace Affy.Core.Framework.Setup;

public static class AppDomainAssemblyScanningExtensions
{
    public static List<Assembly> GetIncludedAssemblies (this AppDomain appDomain)
    {
        return appDomain.GetAssemblies()
            .Where(assembly => assembly.GetCustomAttributes().OfType<IncludeAssemblyAttribute>().Any())
            .ToList();
    }
}