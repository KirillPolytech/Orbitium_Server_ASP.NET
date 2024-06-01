using BackEndForGame.Contracts;
using BackEndForGame.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEndForGame.Controllers
{
    public class PlayerController : ControllerBase
    {
        private PlayerService _playerService;
        public PlayerController(PlayerService service)
        {
            _playerService = service;
        }

        [HttpGet("GetPlayerData")]
        [Authorize]
        public ActionResult<PlayerData?> GetPlayerData()
        {
            string? uid = User?.Identities?.FirstOrDefault()?.Claims.ToList()[0].Value;

            if (uid == null)
                return BadRequest();

            PlayerData? data = _playerService.GetPlayerData(new Guid(uid));

            if (data == null)
                return BadRequest();

            return new ActionResult<PlayerData?>(data);
        }

        [HttpPost("UpdateStatistic")]
        [Authorize]
        public ActionResult UpdateStatistic([FromBody] PlayerData data)
        {
            string? uid = User?.Identities?.FirstOrDefault()?.Claims?.ToList().FirstOrDefault()?.Value;

            if (uid == null)
                return NotFound();

            bool? result = _playerService.UpdatePlayerStatistic(data);

            if (result == false)
                return BadRequest();

            return Ok();
        }
    }
}
