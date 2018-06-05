using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Joonasw.AzureAdApiSample.Api.Authorization
{
    public class PermissionRequirementHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            if (requirement.DelegatedPermissions.Any(p =>
                context.User.HasClaim(Constants.ScopeClaimType, p)))
            {
                // TODO: You may also need to check calling user roles

                //Caller has one of the allowed delegated permissions
                context.Succeed(requirement);
            }
            else if(requirement.ApplicationPermissions.Any(p =>
                context.User.HasClaim(ClaimTypes.Role, p)))
            {
                //Caller has one of the allowed application permissions
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
