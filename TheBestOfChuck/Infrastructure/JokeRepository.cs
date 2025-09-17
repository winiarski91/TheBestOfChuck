namespace TheBestOfChuck.Infrastructure;

using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TheBestOfChuck.Domain.Models;

public class JokeRepository(JokeDbContext dbContext, ILogger<JokeRepository> logger) : IJokeRepository
{
    public async Task SaveJokeAsync(Joke joke)
    {
        logger.LogDebug("Saving joke with ID {Id}", joke.Id);
        await dbContext.Jokes.AddAsync(joke);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Successfully saved joke with ID {Id}", joke.Id);
    }

    public async Task<bool> ExistsByHashAsync(string hash) => await dbContext.Jokes.AnyAsync(j => j.ValueHash == hash);

}
