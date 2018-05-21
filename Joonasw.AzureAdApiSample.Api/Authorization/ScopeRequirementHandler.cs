using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Joonasw.AzureAdApiSample.Api.Authorization
{
    public class ScopeRequirementHandler : AuthorizationHandler<ScopeRequirement>
    {
        private const string ClaimScope = "http://schemas.microsoft.com/identity/claims/scope";

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ScopeRequirement requirement)
        {
            string[] userScopes =
                context.User.FindFirstValue(ClaimScope)?.Split(' ', StringSplitOptions.RemoveEmptyEntries) ??
                new string[0];
            if (userScopes.Contains(requirement.RequiredScopeValue))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
