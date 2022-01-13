using HotChocolate.Language;

namespace Affy.Core.Framework.Cache;

public static class CacheSetupExtensions
{
    public static IServiceCollection AddCache (this IServiceCollection services)
    {
        return services.AddMemoryCache()
            .AddSha256DocumentHashProvider(HashFormat.Hex);
    }
}