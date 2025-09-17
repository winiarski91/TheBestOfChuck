namespace TheBestOfChuck.Tests;

using Microsoft.Extensions.Logging;
using TheBestOfChuck.Application.Services;
using TheBestOfChuck.Domain.Interfaces;
using TheBestOfChuck.Domain.Models;

[TestFixture]
public class JokeServiceTests
{
    private Mock<IJokeProvider> _mockJokeProvider;
    private Mock<IJokeRepository> _mockJokeRepository;
    private Mock<ILogger<JokeService>> _mockLogger;
    private JokeService _jokeService;

    [SetUp]
    public void Setup()
    {
        _mockJokeProvider = new Mock<IJokeProvider>();
        _mockJokeRepository = new Mock<IJokeRepository>();
        _mockLogger = new Mock<ILogger<JokeService>>();
        _jokeService = new JokeService(_mockJokeProvider.Object, _mockJokeRepository.Object, _mockLogger.Object);
    }

    [Test]
    public async Task FetchAndSaveJokesAsync_HappyPath_ShouldSaveAllJokes()
    {
        // Arrange
        var jokes = new List<Joke>
        {
            new(Guid.NewGuid(), "Joke 1", "hash1"),
            new(Guid.NewGuid(), "Joke 2", "hash2"),
            new(Guid.NewGuid(), "Joke 3", "hash3")
        };

        _mockJokeProvider.Setup(p => p.GetJokesAsync(3)).ReturnsAsync(jokes);
        _mockJokeRepository.Setup(r => r.ExistsByHashAsync(It.IsAny<string>())).ReturnsAsync(false);

        // Act
        await _jokeService.FetchAndSaveJokesAsync(3);

        // Assert
        _mockJokeRepository.Verify(r => r.SaveJokeAsync(It.IsAny<Joke>()), Times.Exactly(3));
    }

    [Test]
    public async Task FetchAndSaveJokesAsync_LongJoke_ShouldSkipJoke()
    {
        // Arrange
        var longJoke = new string('x', 201); // Joke longer than 200 chars
        var jokes = new List<Joke>
        {
            new(Guid.NewGuid(), "Short joke", "hash1"),
            new(Guid.NewGuid(), longJoke, "hash2"),
            new(Guid.NewGuid(), "Another short joke", "hash3")
        };

        _mockJokeProvider.Setup(p => p.GetJokesAsync(3)).ReturnsAsync(jokes);
        _mockJokeRepository.Setup(r => r.ExistsByHashAsync(It.IsAny<string>())).ReturnsAsync(false);

        // Act
        await _jokeService.FetchAndSaveJokesAsync(3);

        // Assert
        _mockJokeRepository.Verify(r => r.SaveJokeAsync(It.Is<Joke>(j => j.Value == "Short joke")), Times.Once);
        _mockJokeRepository.Verify(r => r.SaveJokeAsync(It.Is<Joke>(j => j.Value == longJoke)), Times.Never);
    }

    [Test]
    public async Task FetchAndSaveJokesAsync_DuplicateJoke_ShouldSkipJoke()
    {
        // Arrange
        var jokes = new List<Joke>
        {
            new(Guid.NewGuid(), "Joke 1", "hash1"),
            new(Guid.NewGuid(), "Joke 2", "hash2"), // This one is a duplicate
            new(Guid.NewGuid(), "Joke 3", "hash3")
        };

        _mockJokeProvider.Setup(p => p.GetJokesAsync(3)).ReturnsAsync(jokes);
            
        // Setup hash2 to be a duplicate
        _mockJokeRepository.Setup(r => r.ExistsByHashAsync("hash1")).ReturnsAsync(false);
        _mockJokeRepository.Setup(r => r.ExistsByHashAsync("hash2")).ReturnsAsync(true);
        _mockJokeRepository.Setup(r => r.ExistsByHashAsync("hash3")).ReturnsAsync(false);

        // Act
        await _jokeService.FetchAndSaveJokesAsync(3);

        // Assert
        _mockJokeRepository.Verify(r => r.SaveJokeAsync(It.Is<Joke>(j => j.ValueHash == "hash1")), Times.Once);
        _mockJokeRepository.Verify(r => r.SaveJokeAsync(It.Is<Joke>(j => j.ValueHash == "hash2")), Times.Never);
        _mockJokeRepository.Verify(r => r.SaveJokeAsync(It.Is<Joke>(j => j.ValueHash == "hash3")), Times.Once);
    }

    [Test]
    public Task FetchAndSaveJokesAsync_RepositoryThrowsException_ShouldContinueAndRethrow()
    {
        // Arrange
        var jokes = new List<Joke>
        {
            new(Guid.NewGuid(), "Joke 1", "hash1"),
            new(Guid.NewGuid(), "Joke 2", "hash2"), // This one will cause an exception
            new(Guid.NewGuid(), "Joke 3", "hash3")
        };
    
        _mockJokeProvider.Setup(p => p.GetJokesAsync(3)).ReturnsAsync(jokes);
        _mockJokeRepository.Setup(r => r.ExistsByHashAsync(It.IsAny<string>())).ReturnsAsync(false);
        _mockJokeRepository.Setup(r => r.SaveJokeAsync(It.Is<Joke>(j => j.ValueHash == "hash2")))
            .ThrowsAsync(new Exception("DB error"));
    
        // Act & Assert
        var ex = ThrowsAsync<AggregateException>(async () => await _jokeService.FetchAndSaveJokesAsync(3));
        That(ex.Message, Does.Contain("Errors occurred while saving jokes"));
    
        // Should try to save all jokes, but one will fail
        _mockJokeRepository.Verify(r => r.SaveJokeAsync(It.Is<Joke>(j => j.ValueHash == "hash1")), Times.Once);
        _mockJokeRepository.Verify(r => r.SaveJokeAsync(It.Is<Joke>(j => j.ValueHash == "hash3")), Times.Once);
        return Task.CompletedTask;
    }
}
