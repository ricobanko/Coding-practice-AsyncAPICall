using APITests;
using APITests.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddJsonFile("./appsettings.json", optional: false, reloadOnChange: true);

try
{
    AppServices? appServices = new AppServices(builder)
    ?.AddConfig()!
    ?.AddServices()!;

    using IHost host = appServices.BuildHost();

    var toDoTestService = host.Services.GetRequiredService<ToDoTestService>();
    var todos = await toDoTestService.GetUserTodoAsync(10);

    foreach (var todo in todos)
    {
        Console.WriteLine($"Todo: {todo.Title}");
    }
} catch (ArgumentNullException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    throw;
}
