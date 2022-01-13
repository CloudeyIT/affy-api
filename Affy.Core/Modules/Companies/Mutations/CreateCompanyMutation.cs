using System.Security.Claims;
using Affy.Core.Framework.Database;
using Affy.Core.Framework.Database.Rules;
using Affy.Core.Framework.GraphQl.Types;
using Affy.Core.Framework.Identity.Attributes;
using Affy.Core.Framework.Identity.Extensions;
using Affy.Core.Modules.Companies.Entities;
using FairyBread;
using FluentValidation;

namespace Affy.Core.Modules.Companies.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class CreateCompanyMutation
{
    public record CreateCompanyInput(string Name, string Slug);
    
    public record CreateCompanyPayload(Guid Id);

    public class CreateCompanyInputValidator : AbstractValidator<CreateCompanyInput>, IRequiresOwnScopeValidator
    {
        public CreateCompanyInputValidator (MainDb db)
        {
            RuleFor(_ => _.Name)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(200);

            RuleFor(_ => _.Slug)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(100)
                .Matches("^[a-z0-9-]+$")
                .Unique(db, (Company company) => company.Slug);
        }
    }

    [Guard]
    public async Task<CreateCompanyPayload> CreateCompany (CreateCompanyInput input, MainDb db, ClaimsPrincipal claimsPrincipal)
    {
        var company = new Company
        {
            Name = input.Name,
            Slug = input.Slug.ToLower(),
            UserId = claimsPrincipal.GetId()!.Value,
        };

        db.Set<Company>().Add(company);
        await db.SaveChangesAsync();
        
        return new CreateCompanyPayload(company.Id);
    }
}