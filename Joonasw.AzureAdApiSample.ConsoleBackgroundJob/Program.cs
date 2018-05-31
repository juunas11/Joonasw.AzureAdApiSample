using System;
using System.Linq;
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
        }

        private static IConfiguration CreateConfig() =>
            new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddUserSecrets<Program>()
                .Build();
    }
}
