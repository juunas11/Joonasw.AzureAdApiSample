using System.Collections.Generic;
using Joonasw.AzureAdApiSample.Api.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Joonasw.AzureAdApiSample.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc(o =>
                {
                    // Requires authentication across the API
                    o.Filters.Add(new AuthorizeFilter(Policies.Default));
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddAuthorization(o =>
            {
                o.AddPolicy(Policies.Default, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    //policy.RequirePermissions(
                    //    delegated: new[] { "user_impersonation" },
                    //    application: new[] { "Todo.Read.All" });
                });
                o.AddPolicy(Policies.ReadTodoItems, policy =>
                {
                    policy.RequirePermissions(
                        delegated: new[] { Scopes.TodosRead, Scopes.TodosReadWrite }, // One of these scopes required for delegated calls
                        application: new[] { AppRoles.TodosRead, AppRoles.TodosReadWrite }); // One of these roles required for application-only calls
                });
                o.AddPolicy(Policies.WriteTodoItems, policy =>
                {
                    policy.RequirePermissions(
                        delegated: new[] { Scopes.TodosReadWrite },
                        application: new[] { AppRoles.TodosReadWrite });
                });
            });

            services
                .AddAuthentication(o =>
                {
                    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(o =>
                {
                    o.Authority = Configuration["Authentication:Authority"];
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidAudiences = new List<string>
                        {
                            Configuration["Authentication:AppIdUri"],
                            Configuration["Authentication:ClientId"]
                        }
                    };
                });
            services.AddSingleton<IClaimsTransformation, AzureAdScopeClaimTransformation>();
            services.AddSingleton<IAuthorizationHandler, PermissionRequirementHandler>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
