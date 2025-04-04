using APITests.models;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace APITests.Services;
public class ToDoService
{
    private readonly HttpClient _httpClient = null;
    private readonly ILogger<ToDoService> _logger = null;


    public ToDoService(HttpClient httpClient, ILogger<ToDoService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<Todo[]> GetToDosAsync(int userId)
    {
        try
        {
            _logger.LogInformation($"Getting todos for user {userId}");

            Todo[]? todos = await _httpClient.GetFromJsonAsync<Todo[]>(
                    $"todos?userId={userId}",
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));

            return todos ?? [];
        } catch (Exception ex)
        {
            _logger.LogError($"Error getting something fun to say: {ex.Message}", ex);
        }

        return [];
    }
}
