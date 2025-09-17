namespace TheBestOfChuck.Domain.Interfaces;

using TheBestOfChuck.Domain.Models;

public interface IJokeProvider
{
    Task<IEnumerable<Joke>> GetJokesAsync(int count);
}
