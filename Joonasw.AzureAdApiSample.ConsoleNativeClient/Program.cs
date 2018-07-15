using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Joonasw.AzureAdApiSample.ConsoleNativeClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IConfiguration config = CreateConfig();
            ClientSettings settings = config.Get<ClientSettings>();
            var todoApiClient = new TodoApiClient(settings);
            await todoApiClient.ListTodosAsync();
            Guid id = await todoApiClient.CreateTodoAsync(new TodoItem
            {
                Text = "Test from Console Native app",
                IsDone = false
            });
            await todoApiClient.ListTodosAsync();
            await todoApiClient.DeleteTodoAsync(id);
            await todoApiClient.ListTodosAsync();
            Console.WriteLine("Press Enter to quit");
            Console.ReadLine();
        }

        private static IConfiguration CreateConfig() =>
            new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddUserSecrets<Program>()
                .Build();
    }
}
