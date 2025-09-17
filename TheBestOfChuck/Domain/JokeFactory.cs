namespace TheBestOfChuck.Domain;

using Interfaces;
using Models;

public class JokeFactory(IHashService hashService) : IJokeFactory
{
    public Joke Create(string value, Guid? id = null)
    {
        var hash = hashService.GenerateHash(value);
        var joke = new Joke(id ?? Guid.NewGuid(), value, hash);
        
        return joke;
    }
}
