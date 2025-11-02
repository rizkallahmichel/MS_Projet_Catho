using System.Net.Http.Json;
using MyApp.Persistence;

namespace MyApp.WebApp.Clients;

public class TodoClient(HttpClient httpClient) : ITodoClient
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<List<TodoItem>> GetTodoItemsAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<TodoItem>>("/api/todo")
            ?? new List<TodoItem>();
    }

    public async Task<TodoItem> CreateTodoItemAsync(TodoItem item)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/todo", item);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<TodoItem>())
               ?? throw new InvalidOperationException("Failed to deserialize todo item.");
    }
}
