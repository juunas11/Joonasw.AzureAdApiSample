namespace Joonasw.AzureAdApiSample.Api.Data
{
    public interface ITodoContextFactory
    {
        /// <summary>
        /// Creates a <see cref="TodoContext"/> that will
        /// have a query filter setup for only the current
        /// user's data if required.
        /// </summary>
        TodoContext CreateContext();
    }
}