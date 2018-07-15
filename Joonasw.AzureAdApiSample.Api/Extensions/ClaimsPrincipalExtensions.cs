using System.Security.Claims;
using Joonasw.AzureAdApiSample.Api.Authorization;

namespace Joonasw.AzureAdApiSample.Api.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Returns true if the call is app-only,
        /// i.e. an app is making the call as itself.
        /// </summary>
        public static bool IsAppOnlyCall(this ClaimsPrincipal user)
        {
            // If caller has a user principal name in the token,
            // it's a delegated call, so an app-only call is one
            // without the claim
            return !user.HasClaim(c => c.Type == ClaimTypes.Upn);
        }

        /// <summary>
        /// Returns the immutable unique id for the caller.
        /// In case of a user, their object id.
        /// In case of an app-only call, the calling app's
        /// service principal's object id.
        /// </summary>
        public static string GetId(this ClaimsPrincipal user)
        {
            return user.FindFirst(Constants.ObjectIdClaimType).Value;
        }
    }
}
