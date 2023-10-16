using Backend.Contexts;
using Backend.Models.Response;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private IUserService _userService;

        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("register-email")]
        public IActionResult RegisterEmail(string email, string pass)
        {
            // password is already hashed and salted on client side
            try
            {
                if (!Utils.IsValidEmail(email))
                    return BadRequest(new ErrorResponse()
                    {
                        ErrorMessage = "Email is in incorrect format."
                    });

                
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse() { ErrorMessage = "Error: " + e.Message });
            }
        }

        /// <summary>
        /// Logs user in and returns a JWT token
        /// </summary>
        /// <param name="email"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthenticateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [HttpPost("login-email")]
        public IActionResult LoginEmail(string email, string pass)
        {
            if (!Utils.IsValidEmail(email))
                return BadRequest(new ErrorResponse()
                {
                    ErrorMessage = "Password or email is incorrect."
                });

            AuthenticateResponse ar = _userService.Challenge(email, pass);
            if (ar == null)
                return Unauthorized(new ErrorResponse()
                {
                    ErrorMessage = "Password or email is incorrect."
                });

            CookieOptions cookieOptions = new CookieOptions()
            {
                IsEssential = true,
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.UtcNow.AddDays(7),
            };
            Response.Cookies.Append("refreshToken", ar.RefreshToken, cookieOptions);
            return Ok(ar);
        }

        /// <summary>
        /// This allows user to stay logged in without having to re-enter their password
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthenticateResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("refresh-token")]
        public IActionResult RefreshToken()
        {
            try
            {
                string refreshToken = Request.Cookies["refreshToken"];
                AuthenticateResponse ar = _userService.RefreshToken(refreshToken);
                if (ar == null)
                    return Unauthorized(new ErrorResponse()
                    {
                        ErrorMessage = "Refresh token doesn't exist."
                    });

                CookieOptions cookieOptions = new CookieOptions()
                {
                    IsEssential = true,
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddDays(7),
                };
                Response.Cookies.Append("refreshToken", ar.RefreshToken, cookieOptions);
                return Ok(ar);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse() { ErrorMessage = "Error: " + e.Message });
            }
        }

    }
}