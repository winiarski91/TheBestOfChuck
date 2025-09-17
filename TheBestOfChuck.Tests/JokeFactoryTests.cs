using TheBestOfChuck.Domain.Interfaces;

namespace TheBestOfChuck.Tests;

[TestFixture]
public class JokeFactoryTests
{
    private Mock<IHashService> _mockHashService;
    private JokeFactory _jokeFactory;

    [SetUp]
    public void Setup()
    {
        _mockHashService = new Mock<IHashService>();
        _jokeFactory = new JokeFactory(_mockHashService.Object);
    }

    [Test]
    public void Create_WithValueAndId_ShouldCreateJokeWithCorrectProperties()
    {
        // Arrange
        var value = "Chuck Norris can divide by zero.";
        var id = Guid.NewGuid();
        var expectedHash = "hash123";

        _mockHashService.Setup(x => x.GenerateHash(value)).Returns(expectedHash);

        // Act
        var joke = _jokeFactory.Create(value, id);

        // Assert
        That(id, Is.EqualTo(joke.Id));
        That(value, Is.EqualTo(joke.Value));
        That(expectedHash, Is.EqualTo(joke.ValueHash));
        _mockHashService.Verify(x => x.GenerateHash(value), Times.Once);
    }

    [Test]
    public void Create_WithValueOnly_ShouldCreateJokeWithNewGuid()
    {
        // Arrange
        var value = "Chuck Norris can divide by zero.";
        var expectedHash = "hash123";

        _mockHashService.Setup(x => x.GenerateHash(value)).Returns(expectedHash);

        // Act
        var joke = _jokeFactory.Create(value);

        // Assert
        That(Guid.Empty, Is.Not.EqualTo(joke.Id));
        That(value, Is.EqualTo(joke.Value));
        That(expectedHash, Is.EqualTo(joke.ValueHash));
        _mockHashService.Verify(x => x.GenerateHash(value), Times.Once);
    }
}
