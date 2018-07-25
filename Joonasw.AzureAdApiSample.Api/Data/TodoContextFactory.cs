using Joonasw.AzureAdApiSample.Api.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Joonasw.AzureAdApiSample.Api.Data
{
    public class TodoContextFactory : ITodoContextFactory
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TodoContextFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public TodoContext CreateContext()
        {
            string userId = null;
            // We assume the call is app-only for the case where this is executed by EF tooling
            // There will not be an HttpContext then, and certainly no user
            if (!(_httpContextAccessor.HttpContext?.User?.IsAppOnlyCall() ?? true))
            {
                //If the call is delegated, grab the user id so we can filter data with it
                userId = _httpContextAccessor.HttpContext.User.GetId();
            }

            //TODO: In a multi-tenant app, grab the tenant id from the caller (delegated or app-only), and give it to the context as well

            //TODO: Inject IConfiguration in constructor and get DB connection string from there, use actual DB, add migrations
            var options = new DbContextOptionsBuilder<TodoContext>()
                .UseInMemoryDatabase("TodoDb")
                .Options;

            return new TodoContext(options, userId);
        }
    }
}
