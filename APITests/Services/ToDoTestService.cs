using APITests.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APITests.Services;
public class ToDoTestService
{
    private readonly ITodoService _todoService = null;

    public ToDoTestService(ITodoService todoService)
    {
        _todoService = todoService;
    }

    public async Task<Todo[]> GetUserTodoAsync(int userId) 
    {
        return await _todoService.GetUserTodoAsync(10);
    }
}
