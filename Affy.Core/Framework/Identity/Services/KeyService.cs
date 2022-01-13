using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Affy.Core.Framework.Identity.Services;

public class KeyService
{
    public static RsaSecurityKey GetRsaKey (string privateKey)
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(privateKey);

        return new RsaSecurityKey(rsa);
    }

    public static SigningCredentials GetSigningCredentials (RsaSecurityKey key)
    {
        return new SigningCredentials(key, SecurityAlgorithms.RsaSha512);
    }
}