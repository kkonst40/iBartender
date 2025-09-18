using FluentValidation;
using iBartender.API.Contracts.Users;
using iBartender.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;


namespace iBartender.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;
        private readonly IWebHostEnvironment _appEnvironment;

        public UsersController(IUsersService usersService, IWebHostEnvironment appEnvironment)
        {
            _usersService = usersService;
            _appEnvironment = appEnvironment;
        }
        [HttpGet]
        public ActionResult GetTestHello()
        {
            return Ok("Hello, World!");
        }

        [Authorize]
        [HttpGet("{userId:guid}")]
        public async Task<ActionResult> GetUser(Guid userId)
        {
            var user = await _usersService.Get(userId);

            if (user == null)
                return NotFound();

            var response = new GetUserResponse(user.Id, user.Login, user.Bio, user.Photo);

            return Ok(response);
        }

        [Authorize]
        [HttpGet("{userId:guid}/subscribers")]
        public async Task<ActionResult> GetSubscribers(Guid userId)
        {
            var users = await _usersService.GetSubscribers(userId);

            var response = users.Select(u => new GetUserResponse(u.Id, u.Login, u.Bio, u.Photo));

            return Ok(response);
        }

        [Authorize]
        [HttpGet("{userId:guid}/subscribtions")]
        public async Task<ActionResult> GetSubscribtions(Guid userId)
        {
            var users = await _usersService.GetSubscribtions(userId);
            var response = users.Select(u => new GetUserResponse(u.Id, u.Login, u.Bio, u.Photo));

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult> Create(
            [FromBody] CreateUserRequest request,
            [FromServices] IValidator<CreateUserRequest> validator)
        {
            var validationResults = await validator.ValidateAsync(request);
            if (!validationResults.IsValid)
                return BadRequest(validationResults.ToDictionary());

            var newUser = await _usersService.Create(request.Login, request.Email, request.Password);
            var response = new GetUserResponse(newUser.Id, newUser.Login, newUser.Bio, newUser.Photo);

            return Ok(response);
        }

        [Authorize]
        [HttpPost("subscribe")]
        public async Task<ActionResult> Subscribe([FromBody] SubscribeRequest request)
        {
            var userId = Guid.Parse(User.FindFirst("id").Value);
            await _usersService.Subscribe(request.UserId, userId);

            return NoContent();
        }

        [Authorize]
        [HttpPost("unsubscribe")]
        public async Task<ActionResult> Unsubscribe([FromBody] SubscribeRequest request)
        {
            var userId = Guid.Parse(User.FindFirst("id").Value);
            await _usersService.Unsubscribe(request.UserId, userId);

            return NoContent();
        }

        [Authorize]
        [HttpPut("login")]
        public async Task<ActionResult> UpdateLogin(
            [FromBody] UpdateUserLoginRequest request,
            [FromServices] IValidator<UpdateUserLoginRequest> validator)
        {
            var validationResults = await validator.ValidateAsync(request);
            if (!validationResults.IsValid)
                return BadRequest(validationResults.ToDictionary());

            var userId = Guid.Parse(User.FindFirst("id").Value);

            var updatedUser = await _usersService.UpdateLogin(
                userId,
                request.NewLogin);

            if (updatedUser == null)
                return NotFound();

            var response = new GetUserResponse(
                updatedUser.Id,
                updatedUser.Login,
                updatedUser.Bio,
                updatedUser.Photo);

            return Ok(response);
        }


        [Authorize]
        [HttpPut("photo")]
        public async Task<ActionResult> UpdatePhoto([FromForm] UpdateUserPhotoRequest request)
        {
            var userId = Guid.Parse(User.FindFirst("id").Value);

            var updatedUser = await _usersService.UpdatePhoto(
                userId,
                request.NewPhoto,
                _appEnvironment.WebRootPath);

            if (updatedUser == null)
                return NotFound();

            var response = new GetUserResponse(
                updatedUser.Id,
                updatedUser.Login,
                updatedUser.Bio,
                updatedUser.Photo);

            return Ok(response);
        }


        [Authorize]
        [HttpPut("password")]
        public async Task<ActionResult> UpdatePassword(
            [FromBody] UpdateUserPasswordRequest request,
            [FromServices] IValidator<UpdateUserPasswordRequest> validator)
        {
            var validationResults = await validator.ValidateAsync(request);
            if (!validationResults.IsValid)
                return BadRequest(validationResults.ToDictionary());

            var userId = Guid.Parse(User.FindFirst("id").Value);

            var updatedUser = await _usersService.UpdatePassword(
                userId,
                request.NewPassword,
                request.NewPasswordConfirm,
                request.OldPassword);

            if (updatedUser == null)
                return NotFound();

            var response = new GetUserResponse(
                updatedUser.Id,
                updatedUser.Login,
                updatedUser.Bio,
                updatedUser.Photo);

            return Ok(response);
        }

        [Authorize]
        [HttpPut("bio")]
        public async Task<ActionResult> UpdateBio(
            [FromBody] UpdateUserBioRequest request,
            [FromServices] IValidator<UpdateUserBioRequest> validator)
        {
            var validationResults = await validator.ValidateAsync(request);
            if (!validationResults.IsValid)
                return BadRequest(validationResults.ToDictionary());

            var userId = Guid.Parse(User.FindFirst("id").Value);

            var updatedUser = await _usersService.UpdateBio(
                userId,
                request.NewBio);

            if (updatedUser == null)
                return NotFound();

            var response = new GetUserResponse(
                updatedUser.Id,
                updatedUser.Login,
                updatedUser.Bio,
                updatedUser.Photo);

            return Ok(response);
        }


        [Authorize]
        [HttpDelete]
        public async Task<ActionResult> Delete()
        {
            var userId = Guid.Parse(User.FindFirst("id").Value);

            await _usersService.Delete(userId);
            return NoContent();
        }
    }
}
