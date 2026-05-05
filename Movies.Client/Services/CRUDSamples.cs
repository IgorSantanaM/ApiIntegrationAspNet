using Movies.Client.Models;
using System.Diagnostics;
using System.Text.Json;

namespace Movies.Client.Services;

public class CRUDSamples : IIntegrationService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CRUDSamples(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }

    public async Task RunAsync()
    {
        await GetResourceAsync();
    }

    public async Task GetResourceAsync()
    {
        using var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri("http://localhost:5001");
        httpClient.Timeout = TimeSpan.FromSeconds(30);
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        httpClient.DefaultRequestHeaders.Accept.Add(
           new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/xml", 0.9));
        var response = await httpClient.GetAsync("/api/movies");

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        var movies = new List<Movie>();

        if (response.Content.Headers.ContentType!.MediaType == "application/json")
        {
            movies = JsonSerializer.Deserialize<List<Movie>>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        else if (response.Content.Headers.ContentType.MediaType == "application/xml")
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<Movie>));
            movies = serializer.Deserialize(new StringReader(content)) as List<Movie>;
        }
    }

    public async Task GetResourceThroughHttpRequestMessageAsync()
    {
        using var httpClient = _httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri("http://localhost:5001");
        httpClient.Timeout = TimeSpan.FromSeconds(30);

        var request = new HttpRequestMessage(HttpMethod.Get, "api/movies");
        httpClient.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        var response = await httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        var movies = JsonSerializer.Deserialize<List<Movie>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
    }
}
