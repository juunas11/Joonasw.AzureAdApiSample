using System;
using System.Threading.Tasks;

namespace Joonasw.AzureAdApiSample.ConsoleNativeClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var todoApiClient = new TodoApiClient();
            await todoApiClient.ListTodosAsync();
            Guid id = await todoApiClient.CreateTodoAsync(new TodoItem
            {
                Text = "Test from Console Native app",
                IsDone = false
            });
            await todoApiClient.ListTodosAsync();
            await todoApiClient.DeleteTodoAsync(id);
            await todoApiClient.ListTodosAsync();
            Console.ReadLine();
        }
    }
}
