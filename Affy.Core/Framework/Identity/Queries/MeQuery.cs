using System.Security.Claims;
using Affy.Core.Framework.GraphQl.Types;
using Affy.Core.Framework.Identity.Attributes;
using Affy.Core.Framework.Identity.Entities;
using Affy.Core.Framework.Identity.Extensions;
using Microsoft.AspNetCore.Identity;

namespace Affy.Core.Framework.Identity.Queries;

[ExtendObjectType(typeof(Query))]
public class MeQuery
{
    [Guard]
    public async Task<User> Me (ClaimsPrincipal claimsPrincipal, [Service] UserManager<User> userManager)
    {
        return await userManager.FindByIdAsync(claimsPrincipal.GetId().ToString());
    }
}