using Microsoft.AspNetCore.Authorization;

namespace Affy.Core.Framework.Identity.Types;

public interface IPolicy
{
    public AuthorizationPolicy? Policy { get; }
}