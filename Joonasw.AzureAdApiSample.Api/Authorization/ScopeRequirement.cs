using Microsoft.AspNetCore.Authorization;

namespace Joonasw.AzureAdApiSample.Api.Authorization
{
    public class ScopeRequirement : IAuthorizationRequirement
    {
        public ScopeRequirement(string requiredScopeValue)
        {
            if (string.IsNullOrEmpty(requiredScopeValue))
            {
                throw new System.ArgumentException("message", nameof(requiredScopeValue));
            }

            RequiredScopeValue = requiredScopeValue;
        }

        public string RequiredScopeValue { get; }
    }
}
