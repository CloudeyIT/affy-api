using Affy.Core.Framework.GraphQl.Types;
using Affy.Core.Framework.Identity.Entities;
using Affy.Core.Framework.Identity.ResultTypes;
using Affy.Core.Framework.Identity.Services;
using HotChocolate.Execution;
using Microsoft.AspNetCore.Identity;

namespace Affy.Core.Framework.Identity.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class LoginWithPasswordMutation
{
    private readonly TokenService _tokenService;

    public LoginWithPasswordMutation (TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task<LoginResult> LoginWithPassword (
        LoginWithPasswordInput input,
        [Service] UserManager<User> userManager
    )
    {
        var user = await userManager.FindByEmailAsync(input.Email);
        if (user is null)
        {
            throw new QueryException("Invalid email or password");
        }

        var result = await userManager.CheckPasswordAsync(user, input.Password);
        if (!result)
        {
            throw new QueryException("Invalid email or password");
        }

        var token = _tokenService.GenerateToken(user);
        return new LoginResult { Token = token };
    }

    public record LoginWithPasswordInput(string Email, string Password);
}