using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace Joonasw.AzureAdApiSample.Api.Authorization
{
    /// <summary>
    /// Splits the scope claim assigned by Azure AD
    /// by spaces so that you can check for a scope with
    /// <code>User.HasClaim(Constants.ScopeClaimType, "scope")</code>,
    /// instead of having to split by space every time.
    /// </summary>
    public class AzureAdScopeClaimTransformation : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var scopeClaims = principal.FindAll(Constants.ScopeClaimType).ToList();
            if (scopeClaims.Count != 1 || !scopeClaims[0].Value.Contains(' '))
            {
                // Caller has no scopes or has multiple scopes (already split)
                // or they have only one scope
                return Task.FromResult(principal);
            }

            Claim claim = scopeClaims[0];
            string[] scopes = claim.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            IEnumerable<Claim> claims = scopes.Select(s => new Claim(Constants.ScopeClaimType, s));

            return Task.FromResult(new ClaimsPrincipal(new ClaimsIdentity(principal.Identity, claims)));
        }
    }
}
