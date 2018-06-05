using System;

namespace Joonasw.AzureAdApiSample.ConsoleBackgroundJob
{
    public class TodoItem
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public bool IsDone { get; set; }
    }
}