using APITests;
using APITests.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Polly.Retry;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddJsonFile("./appsettings.json", optional: false, reloadOnChange: true);
builder.Services.Configure<HttpClientSettings>(builder.Configuration.GetSection("HttpClientSettings"));

var serviceProvider = builder.Services.BuildServiceProvider();
var httpClientSettings = serviceProvider.GetRequiredService<IOptions<HttpClientSettings>>().Value;

AppServices? appServices = new AppServices(builder)
    ?.AddConfig()!
    ?.AddServices()!
    ?.AddPollyResilience();

using IHost host = builder.Build();

AsyncRetryPolicy asyncRetryPolicy = host.Services.GetRequiredService<AsyncRetryPolicy>();
ITodoService todoService = host.Services.GetRequiredService<ITodoService>();

var todos = await asyncRetryPolicy.ExecuteAsync(() => todoService.GetUserTodoAsync(2));

foreach (var todo in todos)
{
    Console.WriteLine($"Todo: {todo.Title}");
}
