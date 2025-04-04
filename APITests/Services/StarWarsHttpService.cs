namespace APITests.Services;
public class StarWarsHttpService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public StarWarsHttpService(IHttpClientFactory httpClientFactory)
    {
        this._httpClientFactory = httpClientFactory;
    }

    public async Task<string> GetPeopleAsync()
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync("people");
        
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return content;
    }
}
