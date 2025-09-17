namespace TheBestOfChuck.Application.Services;

using Microsoft.Extensions.Logging;
using TheBestOfChuck.Application.Interfaces;
using TheBestOfChuck.Domain.Interfaces;

public class JokeService(
    IJokeProvider jokeProvider,
    IJokeRepository jokeRepository,
    ILogger<JokeService> logger)
    : IJokeService
{
    public async Task FetchAndSaveJokesAsync(int count)
    {
        logger.LogInformation("Fetching {Count} jokes", count);
        var jokes = await jokeProvider.GetJokesAsync(count);
        var savedCount = 0;
        var errors = new List<Exception>();

        foreach (var joke in jokes)
        {
            if (joke.Value.Length > 200)
            {
                logger.LogWarning("Skipping joke exceeding 200 characters: {JokeValue}", joke.Value);
                continue;
            }

            try
            {
                if (await jokeRepository.ExistsByHashAsync(joke.ValueHash))
                {
                    logger.LogWarning("Skipping duplicate joke with hash: {Hash}", joke.ValueHash);
                    continue;
                }

                await jokeRepository.SaveJokeAsync(joke);
                savedCount++;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error saving joke with value: {JokeValue}", joke.Value);
                errors.Add(ex);
            }
        }

        logger.LogInformation("Successfully saved {SavedCount} out of {TotalCount} jokes", savedCount, jokes.Count());
    
        if (errors.Any())
        {
            throw new AggregateException("Errors occurred while saving jokes", errors);
        }
    }
}
