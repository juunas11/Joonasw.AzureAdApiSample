using Microsoft.AspNetCore.Authorization;

namespace Joonasw.AzureAdApiSample.Api.Authorization
{
    public static class AuthorizationPolicyBuilderExtensions
    {
        public static void RequirePermissions(
            this AuthorizationPolicyBuilder builder,
            string[] delegated,
            string[] application = null,
            string[] userRoles = null)
        {
            builder.Requirements.Add(new PermissionRequirement
            {
                DelegatedPermissions = delegated,
                ApplicationPermissions = application ?? new string[0],
                UserRoles = userRoles ?? new string[0]
            });
        }
    }
}
