using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Affy.Core.Framework.Identity.Extensions;
using Affy.Core.Framework.Identity.ResultTypes;
using FluentAssertions;
using GraphQL;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;

namespace Affy.Test.Framework.Identity;

[TestFixture]
public class LoginTests : IntegrationFixture
{
    [Test]
    public async Task Can_Login_With_Correct_Password ()
    {
        var password = RandomPassword;
        var user = await CreateUser(password: password);

        var request = new GraphQLRequest
        {
            Query = @"
                mutation loginWithPassword($input: LoginWithPasswordInput!) {
                    loginWithPassword(input: $input) {
                        token
                    }
                }",
            Variables = new
            {
                input = new
                {
                    email = user.Email,
                    password,
                },
            },
        };

        var response = await GraphQlClient.SendMutationAsync(
            request,
            () => new { loginWithPassword = new LoginResult() }
        );

        response.Errors.Should().BeNullOrEmpty();
        var result = response.Data.loginWithPassword;
        result.Should().NotBeNull();

        var token = result.Token;
        token.Should().NotBeNull();
        token.Should().NotBeEmpty();

        var claimsPrincipal = new JwtSecurityTokenHandler().ValidateToken(
            token,
            Scope.Resolve<TokenValidationParameters>(),
            out var validatedToken
        );

        claimsPrincipal.Should().NotBeNull();
        claimsPrincipal.GetId().Should().Be(user.Id);
        validatedToken.Should().NotBeNull();
    }

    [Test]
    public async Task Cannot_Login_With_Incorrect_Password ()
    {
        var user = await CreateUser();

        var request = new GraphQLRequest
        {
            Query = @"
                mutation loginWithPassword($input: LoginWithPasswordInput!) {
                    loginWithPassword(input: $input) {
                        token
                    }
                }",
            Variables = new
            {
                input = new
                {
                    email = user.Email,
                    password = "wrongPassword",
                },
            },
        };

        try
        {
            var response = await GraphQlClient.SendMutationAsync(
                request,
                () => new { loginWithPassword = new LoginResult() }
            );
            response.Should().BeNull();
        }
        catch (GraphQLHttpRequestException exception)
        {
            exception.Should().NotBeNull();
        }
    }

    [Test]
    public async Task Cannot_Login_With_Incorrect_Email ()
    {
        var password = RandomPassword;
        var user = await CreateUser(password: password);

        var request = new GraphQLRequest
        {
            Query = @"
                mutation loginWithPassword($input: LoginWithPasswordInput!) {
                    loginWithPassword(input: $input) {
                        token
                    }
                }",
            Variables = new
            {
                input = new
                {
                    email = user.Email.Reverse(),
                    password,
                },
            },
        };

        try
        {
            var response = await GraphQlClient.SendMutationAsync(
                request,
                () => new { loginWithPassword = new LoginResult() }
            );
            response.Should().BeNull();
        }
        catch (GraphQLHttpRequestException exception)
        {
            exception.Should().NotBeNull();
        }
    }
}