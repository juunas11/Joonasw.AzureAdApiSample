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
                // for example:
                //
                // if (requirement.UserRoles.Any(r => context.User.HasClaim(ClaimTypes.Role, r)))
                // {
                //     context.Succeed(requirement);
                // }
                // and of course you would remove the below call to Succeed()

                // Since in this API we always return a user's todo items (for delegated calls),
                // there are no roles and we don't need to check.
                // But if there was a role that allows accessing an administrative endpoint,
                // then you need role checks here.

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
