namespace TheBestOfChuck.Tests;

using TheBestOfChuck.Infrastructure.Utils;

[TestFixture]
public class HashServiceTests
{
    private HashService _hashService;

    [SetUp]
    public void Setup()
    {
        _hashService = new HashService();
    }
    
    [Test]
    public void GenerateHash_ValidInput_ReturnsHash()
    {
        // Arrange
        var input = "This is a test joke";

        // Act
        var hash = _hashService.GenerateHash(input);
        That(hash, Is.Not.Null);
        That(hash, Is.Not.Empty);
    }

    [Test]
    public void GenerateHash_NullInput_ThrowsArgumentNullException()
    {
        // Arrange
        string? input = null;

        // Act & Assert
        Throws<ArgumentNullException>(() => _hashService.GenerateHash(input));
    }

    [Test]
    public void GenerateHash_EmptyInput_ThrowsArgumentNullException()
    {
        // Arrange
        var input = string.Empty;

        // Act & Assert
        Throws<ArgumentNullException>(() => _hashService.GenerateHash(input));
    }

    [Test]
    public void GenerateHash_SameInputs_ReturnsSameHash()
    {
        // Arrange
        var input1 = "Chuck Norris joke";
        var input2 = "Chuck Norris joke";

        // Act
        var hash1 = _hashService.GenerateHash(input1);
        var hash2 = _hashService.GenerateHash(input2);

        // Assert
        That(hash1, Is.EqualTo(hash2));
    }

    [Test]
    public void GenerateHash_DifferentInputs_ReturnsDifferentHash()
    {
        // Arrange
        var input1 = "Chuck Norris joke 1";
        var input2 = "Chuck Norris joke 2";

        // Act
        var hash1 = _hashService.GenerateHash(input1);
        var hash2 = _hashService.GenerateHash(input2);

        // Assert
        That(hash1, Is.Not.EqualTo(hash2));
    }

    [Test]
    public void GenerateHash_NormalizesCasing_ReturnsSameHash()
    {
        // Arrange
        var input1 = "CHUCK norris";
        var input2 = "chuck NORRIS";

        // Act
        var hash1 = _hashService.GenerateHash(input1);
        var hash2 = _hashService.GenerateHash(input2);

        // Assert
        That(hash1, Is.EqualTo(hash2));
    }

    [Test]
    public void GenerateHash_NormalizesWhitespace_ReturnsSameHash()
    {
        // Arrange
        var input1 = "  Chuck Norris  ";
        var input2 = "Chuck Norris";

        // Act
        var hash1 = _hashService.GenerateHash(input1);
        var hash2 = _hashService.GenerateHash(input2);

        // Assert
        That(hash1, Is.EqualTo(hash2));
    }
}
