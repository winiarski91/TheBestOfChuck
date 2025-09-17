namespace TheBestOfChuck.Domain.Interfaces;

using TheBestOfChuck.Domain.Models;

public interface IJokeFactory
{
    public Joke Create(string value, Guid? id = null);
}
