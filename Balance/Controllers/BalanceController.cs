using Balance.Models;
using Balance.Services;
using Microsoft.AspNetCore.Mvc;

namespace Balance.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BalanceController: ControllerBase
    {
        private readonly ILogger<BalanceController> _logger;
        private readonly IBalanceManager _balance;

        public BalanceController(ILogger<BalanceController> logger, IBalanceManager balance)
        {
            _logger = logger;
            _balance = balance;
        }

        [HttpPost(Name = "Pay")]
        public async Task<IActionResult> Pay([FromBody] PlayerBetDTO playerBet)
        {
            if (playerBet.bet <= 0 || playerBet.playerid < 0)
            {
                _logger.LogError("pay validation failed.");
                return BadRequest("pay validation failed.");
            }
            else
            {
                long result = await _balance.DeductBalance(playerBet.playerid, playerBet.bet);
                if (result < 0)
                {
                    _logger.LogError("Pay Failed for id:" + playerBet.playerid);
                    return BadRequest("Pay Failed");
                }
                else
                {
                    _logger.LogInformation("Pay success for id:" + playerBet.playerid);
                    return Ok(new PlayerBalanceDTO(playerBet.playerid, result));
                }
            }
        }

        [HttpPut(Name = "UpdateBalance")]
        public async Task<IActionResult> UpdateBalance([FromBody] PlayerBalanceDTO playerBalance)
        {
            if (playerBalance.balance <= 0 || playerBalance.playerid < 0)
            {
                _logger.LogError("updatebalance validation failed.");
                return BadRequest("updatebalance validation failed.");
            }
            else
            {
                var newPlayerBalance = await _balance.UpdateBalance(playerBalance.playerid, playerBalance.balance);
                if (newPlayerBalance == null)
                {
                    _logger.LogError($"updatebalance failed for id:{playerBalance.playerid}");
                    return BadRequest("updatebalance failed");
                }
                else
                {
                    _logger.LogInformation("Update balance success for id:" + newPlayerBalance.playerid + ", new balance:" + newPlayerBalance.balance);
                    return Ok(new PlayerBalanceDTO(newPlayerBalance.playerid, newPlayerBalance.balance));   // shouldnt expose mongodb id.
                }
            }
        }

    }
}

