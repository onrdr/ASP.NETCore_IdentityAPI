
using DataAccess.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Security.Principal;

namespace Business.ClaimProviders
{
    public class ClaimProvider : IClaimsTransformation
    {
        private readonly UserManager<AppUser> UserManager;

        public ClaimProvider(UserManager<AppUser> userManager)
        {
            UserManager = userManager;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal != null && principal.Identity.IsAuthenticated)
            {
                ClaimsIdentity identity = principal.Identity as ClaimsIdentity;

                AppUser user = await UserManager.FindByNameAsync(identity.Name);

                AddCityClaimIfUserIsNotNull(principal, identity, user);

                AddViolenceClaimIfUserIsNotNull(principal, identity, user);
            }
            return principal;
        }

        #region Functions 
        private static void AddCityClaimIfUserIsNotNull(ClaimsPrincipal principal, ClaimsIdentity identity, AppUser user)
        {
            if (user is not null && user.City is not null && !principal.HasClaim(c => c.Type == "City"))
            {
                Claim cityClaim = new("City", user.City, ClaimValueTypes.String, "Internal");
                identity.AddClaim(cityClaim);
            }
        }

        private static void AddViolenceClaimIfUserIsNotNull(ClaimsPrincipal principal, ClaimsIdentity identity, AppUser user)
        { 
            if (user is not null && user.BirthDay is not null && !principal.HasClaim(c => c.Type == "Violence") && AgeControl(user))
            {
                Claim violenceClaim = new("Violence", true.ToString(), ClaimValueTypes.String, "Internal");
                identity.AddClaim(violenceClaim);
            }
        }

        private static bool AgeControl(AppUser user)
        {
            var today = DateTime.Today;
            var age = today.Year - user.BirthDay?.Year;
            return age >= 18; 
        }
        #endregion
    }
}
