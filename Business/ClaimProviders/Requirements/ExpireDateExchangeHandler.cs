
using Microsoft.AspNetCore.Authorization;

namespace Business.ClaimProviders.Requirements
{
    public class ExpireDateExchangeHandler : AuthorizationHandler<ExpireDateExchangeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ExpireDateExchangeRequirement requirement)
        {
            if (context.User is not null && context.User.Identity is not null)
            {
                var claim = context.User.Claims.Where(x => x.Type == "ExpireDateExchange" && x.Value is not null).FirstOrDefault();

                if (claim is not null)
                {
                    if (DateTime.Now < Convert.ToDateTime(claim.Value))
                    {
                        context.Succeed(requirement);
                        return Task.CompletedTask;
                    }
                    context.Fail();
                }
            }
            return Task.CompletedTask;
        }
    }
}
