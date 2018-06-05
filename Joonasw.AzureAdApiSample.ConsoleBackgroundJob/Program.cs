using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Joonasw.AzureAdApiSample.ConsoleBackgroundJob
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IConfiguration config = CreateConfig();
            JobSettings settings = config.Get<JobSettings>();

            var apiClient = new TodoApiClient(settings);
            await apiClient.ListTodosAsync();
        }

        private static IConfiguration CreateConfig() =>
            new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddUserSecrets<Program>()
                .Build();
    }
}
