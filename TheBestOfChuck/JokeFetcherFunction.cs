namespace TheBestOfChuck;

using Application.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class JokeFetcherFunction(
    IJokeService jokeService,
    ILogger<JokeFetcherFunction> logger,
    IConfiguration configuration)
{
    [Function("FetchJokesPeriodically")]
    public async Task Run([TimerTrigger("%JokeFetcher:Schedule%")] FunctionContext context)
    {
        logger.LogInformation("Joke fetcher function executed at: {Time}", DateTime.UtcNow);
        
        var jokesCount = configuration.GetValue("JokeFetcher:Count", 5);
        
        try
        {
            await jokeService.FetchAndSaveJokesAsync(jokesCount);
            logger.LogInformation("Successfully completed joke fetch operation");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in joke fetcher function");
            throw;
        }
    }
}
