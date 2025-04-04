using APITests.models;
using Refit;

namespace APITests.Services;
public interface ITodoService
{
    [Get("/todos?userId={userId}")]
    Task<Todo[]> GetUserTodoAsync(int userId);
}
