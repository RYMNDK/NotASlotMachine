using Moq;
using Microsoft.AspNetCore.Mvc;
using boxs.Models;
using Microsoft.Extensions.Logging;
using boxs.Controllers;
using boxs.Services;

namespace TestSolution
{
    public class TestboxConfigController
    {
        private readonly Mock<ILogger<boxConfigController>> _loggerMock;
        private readonly Mock<IMachineConfiguration> _configurationMock;
        private readonly boxConfigController _controller;

        public TestboxConfigController()
        {
            _loggerMock = new Mock<ILogger<boxConfigController>>();
            _configurationMock = new Mock<IMachineConfiguration>();
            _controller = new boxConfigController(_loggerMock.Object, _configurationMock.Object);
        }

        [Fact]
        public async Task EditboxConfig_ReturnsOk_WhenUpdateIsSuccessful()
        {
            // Arrange: create a DTO with valid parameters.
            var validDto = new boxMachineDTO(5, 3); // valid parameter (adjust as necessary)

            // Setup the configuration to successfully update.
            _configurationMock
                .Setup(c => c.SetConfigAsync(validDto))
                .ReturnsAsync(true);

            // Act: invoke the endpoint.
            var result = await _controller.EditboxConfig(validDto);

            // Assert: Expect an OkObjectResult with a success message.
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Parameter update successful.", okResult.Value);
        }
    }
}
