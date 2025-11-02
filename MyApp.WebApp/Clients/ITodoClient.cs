using MyApp.Persistence;

namespace MyApp.WebApp.Clients;

public interface ITodoClient
{
    Task<List<TodoItem>> GetTodoItemsAsync();
    Task<TodoItem> CreateTodoItemAsync(TodoItem item);
}
