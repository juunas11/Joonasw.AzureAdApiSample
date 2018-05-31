using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;

namespace Joonasw.AzureAdApiSample.ConsoleNativeClient
{
    public class TodoApiClient
    {
        private static readonly HttpClient Client = new HttpClient();
        private readonly ClientSettings _settings;

        public TodoApiClient(ClientSettings settings)
        {
            _settings = settings;
        }

        public async Task ListTodosAsync()
        {
            using (var req = new HttpRequestMessage(HttpMethod.Get, $"{_settings.ApiBaseUrl}/api/todos"))
            {
                string accessToken = await GetAccessTokenAsync();
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                using (HttpResponseMessage res = await Client.SendAsync(req))
                {
                    res.EnsureSuccessStatusCode();
                    string json = await res.Content.ReadAsStringAsync();
                    List<TodoItem> todos = JsonConvert.DeserializeObject<List<TodoItem>>(json);
                    ListTodosOnConsole(todos);
                }
            }
        }

        private void ListTodosOnConsole(List<TodoItem> todos)
        {
            Console.WriteLine($"---Todos list--- ({todos.Count} items)");
            foreach (TodoItem todo in todos)
            {
                Console.WriteLine($"{todo.Text}: {(todo.IsDone ? "Done" : "Not done")} ({todo.Id})");
            }
        }

        public async Task<Guid> CreateTodoAsync(TodoItem todoItem)
        {
            Console.WriteLine("---Create todo item---");
            using (var req = new HttpRequestMessage(HttpMethod.Post, $"{_settings.ApiBaseUrl}/api/todos"))
            {
                string accessToken = await GetAccessTokenAsync();
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                req.Content = new StringContent(
                    JsonConvert.SerializeObject(todoItem),
                    Encoding.UTF8,
                    "application/json");

                using (HttpResponseMessage res = await Client.SendAsync(req))
                {
                    res.EnsureSuccessStatusCode();
                    string json = await res.Content.ReadAsStringAsync();
                    TodoItem createdTodo = JsonConvert.DeserializeObject<TodoItem>(json);
                    Console.WriteLine($"Created: {createdTodo.Text}: {(createdTodo.IsDone ? "Done" : "Not done")} ({createdTodo.Id})");
                    return createdTodo.Id;
                }
            }
        }

        public async Task DeleteTodoAsync(Guid id)
        {
            Console.WriteLine("---Delete todo item---");
            using (var req = new HttpRequestMessage(HttpMethod.Delete, $"{_settings.ApiBaseUrl}/api/todos/{id}"))
            {
                string accessToken = await GetAccessTokenAsync();
                req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                using (HttpResponseMessage res = await Client.SendAsync(req))
                {
                    res.EnsureSuccessStatusCode();
                    Console.WriteLine($"Todo item deleted with id {id}");
                }
            }
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var context = new AuthenticationContext(_settings.Authority);

            AuthenticationResult result;
            try
            {
                result = await context.AcquireTokenSilentAsync(_settings.ApiResourceUri, _settings.ClientId);
            }
            catch (AdalSilentTokenAcquisitionException)
            {
                DeviceCodeResult deviceCodeResult = await context.AcquireDeviceCodeAsync(_settings.ApiResourceUri, _settings.ClientId);
                Console.WriteLine(deviceCodeResult.Message);
                result = await context.AcquireTokenByDeviceCodeAsync(deviceCodeResult);
            }

            return result.AccessToken;
        }
    }
}