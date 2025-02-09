using Microsoft.AspNetCore.Mvc;
using boxs.Models;
using boxs.Services;

namespace boxs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SpinController : ControllerBase
    {
        private readonly ILogger<SpinController> _logger;
        private readonly IBalanceClient _balance;
        private readonly IMachineConfiguration _configuration;
        private readonly IGenerateSpinResult _generator;
        private readonly IValidateSpinResult _validator;

        public SpinController(ILogger<SpinController> logger, IBalanceClient balance, IMachineConfiguration configuration, 
            IGenerateSpinResult generator, IValidateSpinResult validator)
        {
            _logger = logger;
            _balance = balance;
            _configuration = configuration;
            _generator = generator;
            _validator = validator;
        }

        [HttpPost(Name = "Spin")]
        public async Task<IActionResult> Spin([FromBody] BetDTO playerBet)
        {
            if (playerBet.bet <= 0 || playerBet.playerId < 0)
            {
                _logger.LogError("spin validation failed.");
                return BadRequest("spin validation failed.");
            }

            var newBalance = await _balance.PayAsync(playerBet);
            if (newBalance == -1) {
                // this part is important, we could owe user money.
                _logger.LogError("spin pay failed.");
                return BadRequest("spin pay failed.");
            }

            boxMachine configuration = await _configuration.GetConfigAsync();
            int[,] result = _generator.GetSpinResult(configuration);

            // serialize the result
            String resultCSV = _generator.GenerateCSV(result);

            long wins = _validator.GetTotalWins(result);

            if (wins > 0)
            {
                PlayerBalanceDTO playerBalance = new PlayerBalanceDTO(playerBet.playerId, playerBet.bet * wins);
                var updateBalance = await _balance.UpdateBalanceAsync(playerBalance);
                if (updateBalance == -1)
                {
                    // this part is super important, we owe user money.
                    _logger.LogError("spin win failed.");
                    return BadRequest("spin win failed.");
                }

                return Ok(new WinDTO(playerBet.playerId, playerBalance.balance, updateBalance, resultCSV));
            }
            // no win :(
            else {
                return Ok(new WinDTO(playerBet.playerId, 0, newBalance, resultCSV));
            }

        }
    }
}
