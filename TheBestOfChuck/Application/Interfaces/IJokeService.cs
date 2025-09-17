namespace TheBestOfChuck.Application.Interfaces;

public interface IJokeService
{
    Task FetchAndSaveJokesAsync(int count);
}
