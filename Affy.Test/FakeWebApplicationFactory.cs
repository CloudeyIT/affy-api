using Affy.Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Affy.Test;

public class FakeWebApplicationFactory<T> : WebApplicationFactory<Program>
    where T : class
{
    protected override void ConfigureWebHost (IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing")
            .ConfigureAppConfiguration(
                (_, configurationBuilder) =>
                {
                    configurationBuilder
                        .AddJsonFile("appsettings.json", true)
                        .AddJsonFile("appsettings.Testing.json", true)
                        .AddJsonFile("appsettings.Testing.Local.json", true)
                        .AddEnvironmentVariables("APP_");
                }
            );
    }
}