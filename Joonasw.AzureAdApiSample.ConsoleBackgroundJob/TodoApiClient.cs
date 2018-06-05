using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;

namespace Joonasw.AzureAdApiSample.ConsoleBackgroundJob
{
    public class TodoApiClient
    {
        private static readonly HttpClient Client = new HttpClient();
        private readonly JobSettings _settings;

        public TodoApiClient(JobSettings settings)
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

        private async Task<string> GetAccessTokenAsync()
        {
            var context = new AuthenticationContext(_settings.Authority);
            var credentials = new ClientCredential(_settings.ClientId, _settings.ClientSecret);

            AuthenticationResult result = await context.AcquireTokenAsync(
                _settings.ApiResourceUri,
                credentials);
            return result.AccessToken;
        }

        private void ListTodosOnConsole(List<TodoItem> todos)
        {
            Console.WriteLine($"---Todos list--- ({todos.Count} items)");
            foreach (TodoItem todo in todos)
            {
                Console.WriteLine($"{todo.Text}: {(todo.IsDone ? "Done" : "Not done")} ({todo.Id})");
            }
        }
    }
}