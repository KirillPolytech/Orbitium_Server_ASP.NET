using BackEndForGame.Contracts;
using BackEndForGame.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEndForGame.Controllers
{
    public class LeaderBoardController : ControllerBase
    {
        private LeaderBoardService _leaderBoardService;
        public LeaderBoardController(LeaderBoardService service)
        {
            _leaderBoardService = service;
        }

        [HttpGet("GetLeaderBoards")]
        [AllowAnonymous]
        public ActionResult< List<LeaderBoardData> > GetLeaderBoard()
        {
            return new ActionResult< List<LeaderBoardData> >( _leaderBoardService.GetLeaderBoards() );
        }

        [HttpPost("SetRecord")]
        [Authorize]
        public ActionResult SetRecord()
        {
            string? uid = User?.Identities?.FirstOrDefault()?.Claims.ToList()[0].Value;

            if (uid == null)
                return BadRequest();

            if (_leaderBoardService.SetNewRecord(new Guid(uid)) == true)
                return Ok();
            else
                return NotFound();
        }
    }
}
