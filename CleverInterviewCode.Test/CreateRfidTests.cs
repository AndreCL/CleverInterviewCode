using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

public class CreateRfidTests
{
    private readonly Mock<ILogger> mockLogger;

    public CreateRfidTests()
    {
        mockLogger = new Mock<ILogger>();
    }

    private HttpRequest CreateHttpRequest(string body)
    {
        var context = new DefaultHttpContext();
        var request = context.Request;
        request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
        return request;
    }

    [Fact]
    public async Task CreateRfid_ReturnsOkObjectResult_WithValidRfid()
    {
        // Arrange
        string requestBody = "1234";
        var request = CreateHttpRequest(requestBody);
        var logger = mockLogger.Object;

        // Act
        var response = await CreateRfid.Run(request, logger);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(response);
        Assert.Equal($"RFID tag {requestBody} has been created.", okResult.Value);
    }

    [Fact]
    public async Task CreateRfid_ReturnsBadRequestObjectResult_WithEmptyRfid()
    {
        // Arrange
        string requestBody = "";
        var request = CreateHttpRequest(requestBody);
        var logger = mockLogger.Object;

        // Act
        var response = await CreateRfid.Run(request, logger);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(response);
        Assert.Equal("Please pass a valid RFID tag in the request body.", badRequestResult.Value);
    }
}