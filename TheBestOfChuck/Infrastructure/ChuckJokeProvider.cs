namespace TheBestOfChuck.Infrastructure;

using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TheBestOfChuck.Domain.Models;

public class ChuckJokeProvider(
    HttpClient httpClient,
    IJokeFactory jokeFactory,
    ILogger<ChuckJokeProvider> logger,
    IConfiguration configuration)
    : IJokeProvider
{
    private readonly string _apiKey = configuration["ChuckNorrisApi:ApiKey"];
    private readonly string _apiUrl = configuration["ChuckNorrisApi:Url"];

    public async Task<IEnumerable<Joke>> GetJokesAsync(int count)
    {
        var jokes = new List<Joke>();
        
        try
        {
            for (var i = 0; i < count; i++)
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(_apiUrl),
                    Headers =
                    {
                        { "X-RapidAPI-Key", _apiKey },
                        { "X-RapidAPI-Host", "matchilling-chuck-norris-jokes-v1.p.rapidapi.com" },
                        { "accept", "application/json" }
                    }
                };

                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                
                var jokeJson = await response.Content.ReadAsStringAsync();
                var jokeData = JsonSerializer.Deserialize<ChuckNorrisApiResponse>(jokeJson);
                jokes.Add(jokeFactory.Create(jokeData.Value));
            }
            
            return jokes;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching jokes from Chuck Norris API");
            throw;
        }
    }
    
    private class ChuckNorrisApiResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
        
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}

