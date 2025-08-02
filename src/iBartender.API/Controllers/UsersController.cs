using iBartender.API.Contracts.Users;
using iBartender.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace iBartender.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _userService;
        private readonly IWebHostEnvironment _appEnvironment;

        public UsersController(IUsersService usersService, IWebHostEnvironment appEnvironment)
        {
            _userService = usersService;
            _appEnvironment = appEnvironment;
        }

        [Authorize]
        [HttpGet("{userId:guid}")]
        public async Task<ActionResult> GetUser(Guid userId)
        {
            var user = await _userService.Get(userId);

            if (user == null)
                return NotFound();

            var response = new GetUserResponse(user.Id, user.Login, user.Bio, user.Photo);

            return Ok(response);
        }

        [Authorize]
        [HttpGet("{userId:guid}/subscribers")]
        public async Task<ActionResult> GetSubscribers(Guid userId)
        {
            var users = await _userService.GetSubscribers(userId);

            var response = users.Select(u => new GetUserResponse(u.Id, u.Login, u.Bio, u.Photo));

            return Ok(response);
        }

        [Authorize]
        [HttpGet("{userId:guid}/subscribtions")]
        public async Task<ActionResult> GetSubscribtions(Guid userId)
        {
            var users = await _userService.GetSubscribtions(userId);
            var response = users.Select(u => new GetUserResponse(u.Id, u.Login, u.Bio, u.Photo));

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateUserRequest request)
        {
            var newUser = await _userService.Create(request.login, request.email, request.password);
            var response = new GetUserResponse(newUser.Id, newUser.Login, newUser.Bio, newUser.Photo);

            return Ok(response);
        }

        [Authorize]
        [HttpPost("subscribe")]
        public async Task<ActionResult> Subscribe([FromBody] SubscribeRequest request)
        {
            var userId = Guid.Parse(User.FindFirst("id").Value);
            await _userService.Subscribe(request.userId, userId);

            return NoContent();
        }

        [Authorize]
        [HttpPost("unsubscribe")]
        public async Task<ActionResult> Unsubscribe([FromBody] SubscribeRequest request)
        {
            var userId = Guid.Parse(User.FindFirst("id").Value);
            await _userService.Unsubscribe(request.userId, userId);

            return NoContent();
        }

        [Authorize]
        [HttpPut("login")]
        public async Task<ActionResult> UpdateLogin([FromBody] UpdateUserLogin request)
        {
            var userId = Guid.Parse(User.FindFirst("id").Value);

            var updatedUser = await _userService.UpdateLogin(
                userId,
                request.newLogin);

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
        public async Task<ActionResult> UpdatePhoto([FromForm] UpdateUserPhoto request)
        {
            var userId = Guid.Parse(User.FindFirst("id").Value);

            var updatedUser = await _userService.UpdatePhoto(
                userId,
                request.newPhoto,
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
        public async Task<ActionResult> UpdatePassword([FromBody] UpdateUserPassword request)
        {
            var userId = Guid.Parse(User.FindFirst("id").Value);

            var updatedUser = await _userService.UpdatePassword(
                userId,
                request.newPassword,
                request.newPasswordConfirm,
                request.oldPassword);

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

            await _userService.Delete(userId);
            return NoContent();
        }
    }
}
