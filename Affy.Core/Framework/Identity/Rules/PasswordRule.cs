using System.Text.RegularExpressions;
using FluentValidation;

namespace Affy.Core.Framework.Identity.Rules;

public static class PasswordRule
{
    public static IRuleBuilderOptions<T, string> StrongPassword<T> (this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotEmpty()
            .MinimumLength(10)
            .MaximumLength(250)
            .Must(password => Regex.IsMatch(password, "\\W"))
            .WithMessage("Password must contain at least 1 special character")
            .Must(password => Regex.IsMatch(password, "[A-Z]"))
            .WithMessage("Password must contain at least 1 uppercase character")
            .Must(password => Regex.IsMatch(password, "[a-z]"))
            .WithMessage("Password must contain at least 1 lowercase character")
            .Must(password => Regex.IsMatch(password, "[0-9]"))
            .WithMessage("Password must contain at least 1 digit");
    }
}