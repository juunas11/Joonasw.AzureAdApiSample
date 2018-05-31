using System;
using System.Collections.Generic;
using System.Configuration;
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
        private static readonly string Authority = ConfigurationManager.AppSettings["AzureAd:Authority"];
        private static readonly string ApiResourceUri = ConfigurationManager.AppSettings["AzureAd:ApiResourceUri"];
        private static readonly string ClientId = ConfigurationManager.AppSettings["AzureAd:ClientId"];
        private static readonly Uri RedirectUri = new Uri(ConfigurationManager.AppSettings["AzureAd:RedirectUri"]);
        private static readonly string ApiBaseUrl = ConfigurationManager.AppSettings["AzureAd:ApiBaseUrl"];
        private static readonly HttpClient Client = new HttpClient();

        public async Task ListTodosAsync()
        {
            using (var req = new HttpRequestMessage(HttpMethod.Get, $"{ApiBaseUrl}/api/todos"))
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
            using (var req = new HttpRequestMessage(HttpMethod.Post, $"{ApiBaseUrl}/api/todos"))
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
            using (var req = new HttpRequestMessage(HttpMethod.Delete, $"{ApiBaseUrl}/api/todos/{id}"))
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
            var context = new AuthenticationContext(Authority);

            var result = await context.AcquireTokenAsync(
                ApiResourceUri,
                ClientId,
                RedirectUri,
                new PlatformParameters(PromptBehavior.Auto));
            return result.AccessToken;
        }
    }
}