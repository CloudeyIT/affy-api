using System;
using System.Net.Http;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Bogus;
using Affy.Core.Framework.Database;
using Affy.Core.Framework.Identity.Entities;
using Affy.Core.Framework.Identity.Services;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Routing;
using PasswordGenerator;
using WGVMeetinstrumenten.Api;

namespace Affy.Test;

public abstract class IntegrationFixture
{
    protected readonly FakeWebApplicationFactory<ApiModule> Factory;

    protected IntegrationFixture ()
    {
        Factory = new FakeWebApplicationFactory<ApiModule>();
    }

    protected HttpClient HttpClient => Factory.CreateClient(
        new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
        }
    );

    protected ILifetimeScope Scope => Factory.Services.GetAutofacRoot().BeginLifetimeScope();
    protected MainDb Database => Scope.Resolve<MainDb>();
    protected LinkGenerator LinkGenerator => Scope.Resolve<LinkGenerator>();
    protected Faker Faker => new();

    protected string RandomPassword => new Password(true, true, true, true, 16).Next();

    protected GraphQLHttpClient GraphQlClient => new(
        new GraphQLHttpClientOptions
        {
            EndPoint = new Uri("https://localhost/graphql"),
        },
        new NewtonsoftJsonSerializer(),
        HttpClient
    );

    protected async Task<string> GetTokenForUser (string email)
    {
        var user = await Scope.Resolve<UserManager<User>>().FindByEmailAsync(email);
        return GetTokenForUser(user);
    }

    protected string GetTokenForUser (User user)
    {
        return Scope.Resolve<TokenService>().GenerateToken(user);
    }

    protected async Task<User> CreateUser (
        string? email = null,
        string? password = null,
        string? name = null,
        string[]? roles = null
    )
    {
        var username = email ?? Faker.Internet.Email();
        var user = new User
        {
            Email = username,
            UserName = username,
            Name = name ?? Faker.Name.FullName(),
        };

        var userManager = Scope.Resolve<UserManager<User>>();
        var result = await userManager.CreateAsync(user, password ?? RandomPassword);
        if (!result.Succeeded) throw new Exception("Failed to create user");

        await userManager.AddToRolesAsync(user, roles ?? new[] { Role.User });

        return user;
    }
}