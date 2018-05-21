using System;

namespace Joonasw.AzureAdApiSample.Api.Data
{
    public class TodoItem
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public bool IsDone { get; set; }
    }
}
