using Microsoft.EntityFrameworkCore;

namespace MyApp.Persistence;

public class MyAppDbContext(DbContextOptions<MyAppDbContext> options) : DbContext(options)
{
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TodoItemConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
