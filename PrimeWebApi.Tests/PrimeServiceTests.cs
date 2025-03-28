using Microsoft.Extensions.Logging;
using Moq;
using PrimeWebApi.Services;
using ProtoBuf.Meta;
using System.Net;
using WireMock.Server;

namespace PrimeWebApi.Tests
{
    public class PrimeServiceErrorTests
    {
        private readonly PrimeService _primeService;
        private readonly WireMockServer _mockServer;
        private readonly Mock<ILogger<PrimeService>> _mockLogger = new();

        public PrimeServiceErrorTests()
        {
            _mockServer = WireMockServer.Start();
            var httpClient = new HttpClient { BaseAddress = new Uri(_mockServer.Urls[0]) };
            _primeService = new PrimeService(httpClient, _mockLogger.Object);
        }

        [Fact]
        public async Task GenerateRandomNumberAsync_ReturnsInRange()
        {
            // Act
            int result = await _primeService.GenerateRandomNumberAsync(1, 100);

            // Assert
            Assert.InRange(result, 1, 100);
            _mockLogger.VerifyLog(LogLevel.Information, "Generating random number");
        }

        [Fact]
        public async Task GenerateRandomNumberAsync_InvalidRange_Throws()
        {
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                () => _primeService.GenerateRandomNumberAsync(100, 1)
            );
        }

        [Fact]
        public void CheckNumberType_Negative_ReturnsNegative()
        {
            var result = _primeService.CheckNumberType(-5);
            Assert.Equal("Negative", result);
        }

        [Fact]
        public void GetDayName_3_ReturnsWednesday()
        {
            var result = _primeService.GetDayName(3);
            Assert.Equal("Wednesday", result);
        }

        [Fact]
        public async Task GetTodoTitleAsync_ValidId_ReturnsTitle()
        {
            // Arrange: Mock API response
            _mockServer
                .Given(WireMock.RequestBuilders.Request.Create()
                    .WithPath("/todos/1")
                    .UsingGet())
                .RespondWith(WireMock.ResponseBuilders.Response.Create()
                    .WithStatusCode(HttpStatusCode.OK)
                    .WithBodyAsJson(new { userId = 1, id=1, title = "Test todo", completed = false }));

            // Act
            var result = await _primeService.GetTodoTitleAsync(1);

            // Assert
            Assert.Equal("Test todo", result);
        }

        [Fact]
        public void GetDayName_InvalidDay_ThrowsAndLogs()
        {
            // Act & Assert
            var ex = Assert.Throws<ArgumentOutOfRangeException>(
                () => _primeService.GetDayName(8)
            );

            // Verify logging
            _mockLogger.VerifyLog(LogLevel.Warning, "Invalid day number: 8");
            Assert.Contains("Day must be 1-7", ex.Message);
        }

        [Fact]
        public async Task GetTodoTitleAsync_ApiFails_ThrowsAndLogs()
        {
            // Arrange: Mock 500 error
            _mockServer
                .Given(WireMock.RequestBuilders.Request.Create()
                    .WithPath("/todos/1")
                    .UsingGet())
                .RespondWith(WireMock.ResponseBuilders.Response.Create()
                    .WithStatusCode(HttpStatusCode.InternalServerError));

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(
                () => _primeService.GetTodoTitleAsync(1)
            );

            // Verify error was logged
            _mockLogger.VerifyLog(LogLevel.Error, "API call failed for todo ID: 1");
        }
    }

    // Test helper for verifying logs
    public static class LoggerTestExtensions
    {
        public static void VerifyLog<T>(this Mock<ILogger<T>> loggerMock, LogLevel level, string messageFragment)
        {
            loggerMock.Verify(
                x => x.Log(
                    level,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains(messageFragment)),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                Times.AtLeastOnce);
        }
    }
}