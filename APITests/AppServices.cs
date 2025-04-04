using APITests.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using Refit;

namespace APITests;
public class AppServices
{
    private readonly HostApplicationBuilder _hostApplicationBuilder;

    public AppServices(HostApplicationBuilder hostApplicationBuilder)
    {
        _hostApplicationBuilder = hostApplicationBuilder;
    }

    public AppServices? AddServices() 
    {
        if (_hostApplicationBuilder == null)
        {
            throw new ArgumentNullException(nameof(_hostApplicationBuilder));
        }

        _hostApplicationBuilder.Services.Configure<HttpClientSettings>(
        _hostApplicationBuilder.Configuration.GetSection("HttpClientSettings"));

        _hostApplicationBuilder.Services.AddRefitClient<ITodoService>().ConfigureHttpClient((serviceProvider, client) =>
        {
                var httpClientSettings = serviceProvider.GetRequiredService<IOptions<HttpClientSettings>>().Value;
                client.BaseAddress = new Uri(httpClientSettings.BaseUrl);
                client.DefaultRequestHeaders.UserAgent.ParseAdd(httpClientSettings.UserAgent);
        });

        _hostApplicationBuilder.Services.AddScoped<ToDoTestService>();

        return this;
    }

    public AppServices? AddPollyResilience()
    {
        var sleepsBeetweenRetries = new List<TimeSpan>{ 
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(5),
            TimeSpan.FromSeconds(10),
            TimeSpan.FromSeconds(15)};

        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                sleepDurations: sleepsBeetweenRetries,
                onRetry: (ex, span, retry, _) =>
                {
                    Console.WriteLine(ex);

                    var backgroundColour = Console.BackgroundColor;
                    var foregroundColour = Console.ForegroundColor;

                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.Black;

                    Console.Out.WriteLineAsync($"Error recorded at {DateTime.Now:HH:mm:ss}, will retry. {retry}");

                    Console.BackgroundColor = backgroundColour;
                    Console.ForegroundColor = foregroundColour;
                }
            );

        _ = _hostApplicationBuilder.Services.AddSingleton<AsyncRetryPolicy>(retryPolicy);

        return this;
    }

    public AppServices? AddConfig()
    {
        if (_hostApplicationBuilder == null)
        {
            throw new ArgumentNullException(nameof(_hostApplicationBuilder));
        }

        _hostApplicationBuilder.Configuration.AddJsonFile("./appsettings.json", 
            optional: false, reloadOnChange: true);

        _hostApplicationBuilder.Services.Configure<HttpClientSettings>(
            _hostApplicationBuilder.Configuration.GetSection("HttpClientSettings"));

        return this;
    }

    public IHost BuildHost()
    {
        return _hostApplicationBuilder.Build();
    }
}
