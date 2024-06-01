using Microsoft.AspNetCore.Mvc;
using BackEndForGame.Contracts;
using BackEndForGame.Services;
using Microsoft.AspNetCore.Authorization;

namespace BackEndForGame.Configuration
{
    [Route("api/[controller]")]
    [ApiController]

    [Authorize]
    public class UserController : ControllerBase
    {
        private UserService _userService;
        private JwtService _jwtService;
        public UserController(UserService service, JwtService jwtService)
        {
            _userService = service;
            _jwtService = jwtService;
        }

        [HttpPost($"Login")]
        [AllowAnonymous]
        public ActionResult<JwtToken> Login( LoginData data  )
        {
            if (data == null)
                return NotFound();

            JwtToken? token = _userService.Login(data);

            if (token == null)
                return BadRequest();

            return Ok(token);
        }

        [HttpPost($"RegisterUser")]
        [AllowAnonymous]
        public ActionResult<JwtToken> RegisterUser(RegisterData data)
        {
            if (data == null)
                return BadRequest();

            string token = _jwtService.GenerateToken( _userService.CreateUser(data) );

            if (token == null)
                return BadRequest();

            return new JwtToken() { Token = token };
        }

        [HttpDelete($"DeleteUser")]
        [Authorize]
        public ActionResult DeleteUser()
        {
            string? uid = User?.Identities?.FirstOrDefault()?.Claims.ToList()[0].Value;

            if (uid == null)
                return BadRequest();

            _userService.DeleteUser(new Guid(uid));

            return Ok();
        }
    }
}
