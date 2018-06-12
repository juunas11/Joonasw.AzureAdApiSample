using System.Security.Claims;
using Joonasw.AzureAdApiSample.Api.Authorization;

namespace Joonasw.AzureAdApiSample.Api.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Returns true if the call is delegated,
        /// i.e. an app is making the call on behalf
        /// of a user.
        /// </summary>
        public static bool IsDelegatedCall(this ClaimsPrincipal user)
        {
            // If caller has a user principal name in the token,
            // it's a delegated call
            return user.HasClaim(c => c.Type == ClaimTypes.Upn);
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
