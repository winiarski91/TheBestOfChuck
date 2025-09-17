namespace TheBestOfChuck.Domain.Interfaces;

using TheBestOfChuck.Domain.Models;

public interface IJokeRepository
{
    Task SaveJokeAsync(Joke joke);
    Task<bool> ExistsByHashAsync(string hash);
}
