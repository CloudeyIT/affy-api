using GraphQL.Client.Http;
using IdentityModel.Client;

namespace Affy.Test.Testing;

public static class GraphQlClientExtensions
{
    public static GraphQLHttpClient WithToken (this GraphQLHttpClient client, string token)
    {
        client.HttpClient.SetBearerToken(token);

        return client;
    }
}