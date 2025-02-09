using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using boxs.Controllers;
using boxs.Models;
using boxs.Services;

namespace boxs.Tests
{
    public class SpinControllerTests
    {
        [Fact]
        public async Task Spin_ReturnsOkResult_ForValidBet()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<SpinController>>();
            var balanceClientMock = new Mock<IBalanceClient>();
            var configurationMock = new Mock<IMachineConfiguration>();
            var generatorMock = new Mock<IGenerateSpinResult>();
            var validatorMock = new Mock<IValidateSpinResult>();

            var bet = new BetDTO(1, 10);

            balanceClientMock
                .Setup(x => x.PayAsync(bet))
                .ReturnsAsync(100);

            int[,] spinResult = new int[3, 3]
            {
                { 1, 1, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 }
            };
            generatorMock
                .Setup(x => x.GetSpinResult(It.IsAny<boxMachine>()))
                .Returns(spinResult);

            long wins = 2;
            validatorMock
                .Setup(x => x.GetTotalWins(spinResult))
                .Returns(wins);

            var balance = new PlayerBalanceDTO(1, 20);

            balanceClientMock
                .Setup(x => x.UpdateBalanceAsync(balance))
                .ReturnsAsync(120);

            var controller = new SpinController(
                loggerMock.Object,
                balanceClientMock.Object,
                configurationMock.Object,
                generatorMock.Object,
                validatorMock.Object);

            generatorMock
                .Setup(x => x.GetSpinResult(It.IsAny<boxMachine>()))
                .Returns(spinResult);

            generatorMock
                .Setup(x => x.GenerateCSV(spinResult))
                .Returns("1,1,3\r\n4,5,6\r\n7,8,9");

            // Act
            var actionResult = await controller.Spin(bet);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var winDto = Assert.IsType<WinDTO>(okResult.Value);

            Assert.Equal(bet.playerId, winDto.playerId);
            Assert.Equal(20, winDto.win);
            Assert.Equal(100 + 20, winDto.balance);
            Assert.Equal("1,1,3\r\n4,5,6\r\n7,8,9", winDto.resultCSV);
        }

        [Fact]
        public async Task Spin_ReturnsOkResultNoWin_NoWin()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<SpinController>>();
            var balanceClientMock = new Mock<IBalanceClient>();
            var configurationMock = new Mock<IMachineConfiguration>();
            var generatorMock = new Mock<IGenerateSpinResult>();
            var validatorMock = new Mock<IValidateSpinResult>();

            var bet = new BetDTO(1, 10);

            balanceClientMock
                .Setup(x => x.PayAsync(bet))
                .ReturnsAsync(100);

            int[,] spinResult = new int[3, 3]
            {
                { 1, 2, 3 },
                { 4, 5, 6 },
                { 7, 8, 9 }
            };
            generatorMock
                .Setup(x => x.GetSpinResult(It.IsAny<boxMachine>()))
                .Returns(spinResult);

            long wins = 0;
            validatorMock
                .Setup(x => x.GetTotalWins(spinResult))
                .Returns(wins);

            var balance = new PlayerBalanceDTO(1, 20);

            balanceClientMock
                .Setup(x => x.UpdateBalanceAsync(balance))
                .ReturnsAsync(0);

            var controller = new SpinController(
                loggerMock.Object,
                balanceClientMock.Object,
                configurationMock.Object,
                generatorMock.Object,
                validatorMock.Object);

            generatorMock
                .Setup(x => x.GetSpinResult(It.IsAny<boxMachine>()))
                .Returns(spinResult);

            generatorMock
                .Setup(x => x.GenerateCSV(spinResult))
                .Returns("1,2,3\r\n4,5,6\r\n7,8,9");

            // Act
            var actionResult = await controller.Spin(bet);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var winDto = Assert.IsType<WinDTO>(okResult.Value);

            Assert.Equal(bet.playerId, winDto.playerId);
            Assert.Equal(0, winDto.win);
            Assert.Equal(100, winDto.balance);
            Assert.Equal("1,2,3\r\n4,5,6\r\n7,8,9", winDto.resultCSV);
        }


        [Fact]
        public async Task Spin_ReturnsBadRequest_ForInvalidBet()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<SpinController>>();
            var balanceClientMock = new Mock<IBalanceClient>();
            var configurationMock = new Mock<IMachineConfiguration>();
            var generatorMock = new Mock<IGenerateSpinResult>();
            var validatorMock = new Mock<IValidateSpinResult>();

            var bet = new BetDTO(1,-5);

            var controller = new SpinController(
                loggerMock.Object,
                balanceClientMock.Object,
                configurationMock.Object,
                generatorMock.Object,
                validatorMock.Object);

            // Act
            var actionResult = await controller.Spin(bet);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
            Assert.Equal("spin validation failed.", badRequestResult.Value);
        }

    }
}