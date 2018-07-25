using Microsoft.EntityFrameworkCore;

namespace Joonasw.AzureAdApiSample.Api.Data
{
    public class TodoContext : DbContext
    {
        private readonly string _userId;

        public TodoContext(DbContextOptions<TodoContext> options, string userId)
            : base(options)
        {
            _userId = userId;
        }

        public DbSet<TodoItem> TodoItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Setup the default query filter that filters out other users' items if a user id is specified to this context instance
            modelBuilder.Entity<TodoItem>()
                .HasQueryFilter(i => i.UserId == _userId || _userId == null);

            //TODO: In a multi-tenant app, you could implement another query filter for that
            //.HasQueryFilter(i => i.TenantId == _tenantId);
        }
    }
}
