using Microsoft.AspNetCore.Authorization;

namespace Joonasw.AzureAdApiSample.Api.Authorization
{
    public static class AuthorizationPolicyBuilderExtensions
    {
        public static void RequirePermissions(
            this AuthorizationPolicyBuilder builder,
            string[] delegated,
            string[] application,
            string[] userRoles = null)
        {
            builder.Requirements.Add(new PermissionRequirement
            {
                DelegatedPermissions = delegated,
                ApplicationPermissions = application,
                UserRoles = userRoles ?? new string[0]
            });
        }
    }
}
