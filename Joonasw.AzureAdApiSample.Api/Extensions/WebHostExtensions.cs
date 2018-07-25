using System;
using Joonasw.AzureAdApiSample.Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Joonasw.AzureAdApiSample.Api.Extensions
{
    public static class WebHostExtensions
    {
        public static IWebHost AddSeedDataForInMemoryDb(this IWebHost host)
        {
            using (IServiceScope scope = host.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<TodoContext>();
                db.TodoItems.Add(new TodoItem
                {
                    Id = new Guid("9B3731D5-ED6C-4056-BB67-A9F4930AE94B"),
                    Text = "Learn authorization with Azure AD",
                    IsDone = true,
                    UserId = "910e8ad5-b64c-446c-990d-a430651854d1"
                });
                db.TodoItems.Add(new TodoItem
                {
                    Id = new Guid("9B3731D5-ED6C-4056-BB67-A9F4930AE94C"),
                    Text = "Go shopping",
                    IsDone = false,
                    UserId = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaaa"
                });
                db.SaveChanges();
            }
            return host;
        }
    }
}
