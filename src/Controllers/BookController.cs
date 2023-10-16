using System.Security.Claims;
using Backend.Models;
using Backend.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("book")]
    public class BookController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        public BookController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [ProducesResponseType(typeof(BookResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
        [HttpPost("get-branch-books")]
        public IActionResult GetBranchBooks(string branchId)
        {
            if (User.Identity is ClaimsIdentity identity && identity.IsAuthenticated)
            {
                var userClaim = identity.FindFirst("group");
                if (userClaim != null && userClaim.Value == "staff")
                {
                    return Ok();
                }
            }
            return Unauthorized(new ErrorResponse()
            {
                ErrorMessage = "You are not authorized to access this resource."
            });
        }
    }
}