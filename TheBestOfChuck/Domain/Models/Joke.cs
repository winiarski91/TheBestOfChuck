namespace TheBestOfChuck.Domain.Models;

public class Joke
{
    public Guid Id { get; private set; }
    public string Value { get; private set; }
    public string ValueHash { get; private set; }

    public Joke(Guid id, string value, string valueHash)
    {
        Id = id;
        Value = value;
        ValueHash = valueHash;
    }
    
    protected Joke()
    {
    }
}
