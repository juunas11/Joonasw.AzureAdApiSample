using System;

namespace Joonasw.AzureAdApiSample.ConsoleNativeClient
{
    public class TodoItem
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public bool IsDone { get; set; }
    }
}
