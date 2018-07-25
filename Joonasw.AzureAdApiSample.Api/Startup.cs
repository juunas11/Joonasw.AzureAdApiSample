using System.Collections.Generic;
using Joonasw.AzureAdApiSample.Api.Authorization;
using Joonasw.AzureAdApiSample.Api.Data;
using Joonasw.AzureAdApiSample.Api.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
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
                    //To require the basic user_impersonation scope across the API, you can use:
                    //policy.RequirePermissions(
                    //    delegated: new[] { "user_impersonation" },
                    //    application: new string[0]);
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
                    //In a multi-tenant app, make sure the authority is:
                    //o.Authority = "https://login.microsoftonline.com/common";
                    o.Authority = Configuration["Authentication:Authority"];
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidAudiences = new List<string>
                        {
                            Configuration["Authentication:AppIdUri"],
                            Configuration["Authentication:ClientId"]
                        },
                        // In multi-tenant apps you should disable issuer validation:
                        // ValidateIssuer = false,
                        // In case you want to allow only specific tenants,
                        // you can set the ValidIssuers property to a list of valid issuer ids
                        // or specify a delegate for the IssuerValidator property, e.g.
                        // IssuerValidator = (issuer, token, parameters) => {}
                        // the validator should return the issuer string
                        // if it is valid and throw an exception if not
                    };
                });
            services.AddSingleton<IClaimsTransformation, AzureAdScopeClaimTransformation>();
            services.AddSingleton<IAuthorizationHandler, PermissionRequirementHandler>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<ITodoContextFactory, TodoContextFactory>();
            services.AddScoped<TodoContext>(provider =>
            {
                var factory = provider.GetRequiredService<ITodoContextFactory>();
                return factory.CreateContext();
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Very important that this is before MVC (or anything that will require authentication)
            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
