using Moq;
using Moq.Protected;
using System.Net;
using Planday.Schedule.Infrastructure.Providers;

namespace Planday.Schedule.Infrastructure.Tests.Providers
{
    public class PlandayEmployeeProviderTests
    {
        [Theory]
        [InlineData(1, "John Doe", "john@doe.com")]
        [InlineData(9394, "", "")]
        public async Task GetEmployeeAsync_Success(long employeeId, string expectedName, string expectedEmail)
        {
            // Arrange
            var expectedContent = $"{{\r\n    \"name\": \"{expectedName}\",\r\n    \"email\": \"{expectedEmail}\"\r\n}}";

            var messageHandlerMock = new Mock<HttpMessageHandler>();

            messageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(message => message.Method == HttpMethod.Get &&
                                                             message.RequestUri.AbsolutePath == $"/employee/{employeeId}"),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(expectedContent),
                });

            var httpClient = new HttpClient(messageHandlerMock.Object)
            {
                BaseAddress = new Uri("http://myurl.com")
            };
            var factoryMock = new Mock<IHttpClientFactory>();
            factoryMock
                .Setup(f => f.CreateClient(It.Is<string>(name => name == "planday")))
                .Returns(httpClient);

            var service = new PlandayEmployeeProvider(factoryMock.Object);

            // Act
            var actualContent = await service.GetEmployeeAsync(employeeId);

            // Assert
            Assert.Equal(expectedName, actualContent.Name);
            Assert.Equal(expectedEmail, actualContent.Email);
        }
    }
}
