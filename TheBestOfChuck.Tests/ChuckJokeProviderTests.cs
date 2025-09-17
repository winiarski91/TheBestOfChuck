namespace TheBestOfChuck.Tests;

using System.Net;
using Domain.Interfaces;
using Domain.Models;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq.Protected;

[TestFixture]
public class ChuckJokeProviderTests
{
    private Mock<ILogger<ChuckJokeProvider>> _loggerMock;
    private Mock<IConfiguration> _configurationMock;
    private Mock<HttpMessageHandler> _handlerMock;
    private Mock<IJokeFactory> _jokeFactoryMock;
    private HttpClient _httpClient;
    private ChuckJokeProvider _provider;

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<ChuckJokeProvider>>();
        _configurationMock = new Mock<IConfiguration>();
        _handlerMock = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_handlerMock.Object);
        _jokeFactoryMock = new Mock<IJokeFactory>();

        _configurationMock.Setup(c => c["ChuckNorrisApi:ApiKey"]).Returns("test-api-key");
        _configurationMock.Setup(c => c["ChuckNorrisApi:Url"]).Returns("https://test-api.com/jokes");
        _jokeFactoryMock.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<Guid?>()))
            .Returns((string value, Guid? id) => new Joke(id ?? Guid.NewGuid(), value, "mock-hash-value"));

        _provider = new ChuckJokeProvider(_httpClient, _jokeFactoryMock.Object, _loggerMock.Object, _configurationMock.Object);
    }

    [Test]
    public async Task GetJokesAsync_ReturnsParsedJokes_WhenApiCallSucceeds()
    {
        // Arrange
        const int jokeCount = 2;
        var sampleResponse = @"{""id"":""12345"",""value"":""Chuck Norris test joke"",""created_at"":""2020-01-01T00:00:00.000Z""}";

        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(sampleResponse)
            });

        // Act
        var jokes = await _provider.GetJokesAsync(jokeCount);

        // Assert
        var jokesList = jokes.ToList();
        That(jokesList.Count, Is.EqualTo(jokeCount));
        foreach (var joke in jokesList)
        {
            That(joke.Value, Is.EqualTo("Chuck Norris test joke"));
        }

        // Verify API calls
        _handlerMock.Protected().Verify(
            "SendAsync",
            Times.Exactly(jokeCount),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
    }

    [Test]
    public void GetJokesAsync_ThrowsException_WhenApiCallFails()
    {
        // Arrange
        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("API unavailable"));

        // Act & Assert
        ThrowsAsync<HttpRequestException>(() => _provider.GetJokesAsync(1));

        // Verify error was logged
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
            Times.Once);
    }

    [Test]
    public async Task GetJokesAsync_CorrectlyFormatsHttpRequest()
    {
        // Arrange
        const string apiUrl = "https://test-api.com/jokes";
        const string apiKey = "test-api-key";
        HttpRequestMessage capturedRequest = null;

        _handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((request, _) => capturedRequest = request)
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(@"{""id"":""12345"",""value"":""Test joke"",""created_at"":""2020-01-01T00:00:00.000Z""}")
            });

        // Act
        await _provider.GetJokesAsync(1);

        // Assert
        IsNotNull(capturedRequest);
        That(capturedRequest.Method, Is.EqualTo(HttpMethod.Get));
        That(capturedRequest.RequestUri.ToString(), Is.EqualTo(apiUrl));
        IsTrue(capturedRequest.Headers.Contains("X-RapidAPI-Key"));
        That(capturedRequest.Headers.GetValues("X-RapidAPI-Key").First(), Is.EqualTo(apiKey));
        IsTrue(capturedRequest.Headers.Contains("X-RapidAPI-Host"));
        That(capturedRequest.Headers.GetValues("X-RapidAPI-Host").First(), Is.EqualTo("matchilling-chuck-norris-jokes-v1.p.rapidapi.com"));
    }
}
