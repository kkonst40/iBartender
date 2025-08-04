using FluentValidation;
using iBartender.API.Contracts.Users;
using iBartender.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iBartender.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(
            [FromBody] LoginUserRequest request,
            [FromServices] IValidator<LoginUserRequest> validator)
        {
            var validationResults = await validator.ValidateAsync(request);
            if (!validationResults.IsValid)
                return BadRequest(validationResults.ToDictionary());

            var token = await _authService.Login(request.Email, request.Password);
            HttpContext.Response.Cookies.Append("pechenye", token);

            return NoContent();
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            var userId = Guid.Parse(User.FindFirst("id").Value);

            await _authService.Logout(userId);
            HttpContext.Response.Cookies.Append("pechenye", "");

            return NoContent();
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult> Check()
        {
            return NoContent();
        }

    }
}
