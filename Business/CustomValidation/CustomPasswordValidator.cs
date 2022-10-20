using DataAccess.Models;
using Microsoft.AspNetCore.Identity;

namespace Business.CustomValidation
{
    public class CustomPasswordValidator : IPasswordValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string password)
        {
            List<IdentityError> errors = new();

            if (password.ToLower().Contains(user.UserName.ToLower()))
                errors.Add(new IdentityError() { Code = "PasswordContainsUsername", Description = "Password cannot contain username" });

            if (password.ToLower().Contains("1234"))
                errors.Add(new IdentityError() { Code = "PasswordContains1234", Description = "Password cannot contain consecutive numbers : 1,2,3,4" });

            if (password.ToLower().Contains(user.Email.ToLower()))
                errors.Add(new IdentityError() { Code = "PasswordContainsEmail", Description = "Password cannot contain Email Address" });

            if (errors.Count == 0)
                return Task.FromResult(IdentityResult.Success);

            return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
        }
    }
}