using System.Threading.Tasks;
using FluentAssertions;
using GraphQL;
using NUnit.Framework;
using Snapshooter.NUnit;

namespace Affy.Test.Framework.Identity;

[TestFixture]
public class RegisterUserTests : IntegrationFixture
{
    [Test]
    public async Task Can_Register_User_As_Unauthenticated_User ()
    {
        var request = new GraphQLRequest
        {
            Query = @"
                mutation register {
                    registerUser(
                        input: {
                            email: ""test@example.com"",
                            password: ""Password123!"",
                            firstName: ""Test"",
                            lastName: ""User""
                        }
                    ) {
                        email
                        firstName
                        lastName
                        fullName
                    }
                }
            ",
        };

        var client = GraphQlClient;

        var response = await client
            .SendMutationAsync<object>(request);

        response.Data.Should().MatchSnapshot();
    }
}