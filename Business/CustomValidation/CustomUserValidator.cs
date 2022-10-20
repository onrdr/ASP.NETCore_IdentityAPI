
using DataAccess.Models;
using Microsoft.AspNetCore.Identity;

namespace Business.CustomValidation
{
    public class CustomUserValidator : IUserValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
        {
            List<IdentityError> errors = new();

            if (char.IsDigit(user.UserName[0]))                   
                errors.Add(new IdentityError() { Code = "UsernameFirstCharIsDigit", Description = "First Character should be non-numeric" });

            if (errors.Count == 0)
                return Task.FromResult(IdentityResult.Success);

            return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
        }
    }
}
