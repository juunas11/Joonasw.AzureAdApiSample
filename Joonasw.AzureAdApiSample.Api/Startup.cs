using System.Collections.Generic;
using Joonasw.AzureAdApiSample.Api.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddMvc(o =>
            {
                o.Filters.Add(new AuthorizeFilter("default"));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddAuthorization(o =>
            {
                o.AddPolicy("default", policy =>
                {
                    policy.RequireClaim(Constants.ScopeClaimType, "user_impersonation");
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
                    o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
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
