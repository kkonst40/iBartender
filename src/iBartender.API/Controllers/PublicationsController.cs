using FluentValidation;
using iBartender.API.Contracts.Publications;
using iBartender.Application.Services;
using iBartender.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;


namespace iBartender.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicationsController : ControllerBase
    {
        private readonly IPublicationsService _publicationsService;
        private readonly IWebHostEnvironment _appEnvironment;

        public PublicationsController(IPublicationsService publicationsService, IWebHostEnvironment appEnvironment)
        {
            _publicationsService = publicationsService;
            _appEnvironment = appEnvironment;
        }

        [Authorize]
        [HttpGet("{publicationId:guid}")]
        public async Task<ActionResult<GetPublicationResponse>> Get(Guid publicationId)
        {
            var publication = await _publicationsService.Get(publicationId);

            if (publication == null)
                return NotFound();

            var response = new GetPublicationResponse(
                publication.Id,
                publication.UserId,
                publication.Text,
                publication.Files,
                publication.CreatedAt,
                publication.IsEdited);

            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Create(
            [FromForm] CreatePublicationRequest request,
            [FromServices] IValidator<CreatePublicationRequest> validator)
        {
            var validationResults = await validator.ValidateAsync(request);
            if (!validationResults.IsValid)
                return BadRequest(validationResults.ToDictionary());

            var userId = Guid.Parse(User.FindFirst("id").Value);

            var publication = await _publicationsService.Create(
                userId,
                request.Text,
                request.Files,
                _appEnvironment.WebRootPath);

            var response = new GetPublicationResponse(
                publication.Id,
                publication.UserId,
                publication.Text,
                publication.Files,
                publication.CreatedAt,
                publication.IsEdited);

            return Ok(response);
        }

        [Authorize]
        [HttpPut("{publicationId:guid}/files")]
        public async Task<ActionResult> UpdateFiles(
            Guid publicationId,
            [FromForm] UpdatePublicationFilesRequest request)
        {
            var userId = Guid.Parse(User.FindFirst("id").Value);

            var publication = await _publicationsService.UpdateFiles(
                publicationId,
                request.Files,
                _appEnvironment.WebRootPath,
                userId);

            if (publication == null)
                return NotFound();

            var response = new GetPublicationResponse(
                publication.Id,
                publication.UserId,
                publication.Text,
                publication.Files,
                publication.CreatedAt,
                publication.IsEdited);

            return Ok(response);
        }

        [Authorize]
        [HttpPut("{publicationId:guid}/text")]
        public async Task<ActionResult> UpdateText(
            Guid publicationId,
            [FromForm] UpdatePublicationTextRequest request,
            [FromServices] IValidator<UpdatePublicationTextRequest> validator)
        {
            var validationResults = await validator.ValidateAsync(request);
            if (!validationResults.IsValid)
                return BadRequest(validationResults.ToDictionary());

            var userId = Guid.Parse(User.FindFirst("id").Value);
            
            var publication = await _publicationsService.UpdateText(
                publicationId,
                request.Text,
                userId);

            if (publication == null)
                return NotFound();

            var response = new GetPublicationResponse(
                publication.Id,
                publication.UserId,
                publication.Text,
                publication.Files,
                publication.CreatedAt,
                publication.IsEdited);

            return Ok(response);
        }

        [Authorize]
        [HttpDelete("{publicationId:guid}")]
        public async Task<ActionResult> Delete(Guid publicationId)
        {
            var userId = Guid.Parse(User.FindFirst("id").Value);

            await _publicationsService.Delete(publicationId, userId);

            return NoContent();
        }

        [Authorize]
        [HttpGet("{publicationId:guid}/comments")]
        public async Task<ActionResult> GetComments(Guid publicationId)
        {
            var comments = await _publicationsService.GetComments(publicationId);

            var response = comments.Select(c => new GetCommentResponse(
                c.Id,
                c.PublicationId,
                c.UserId,
                c.Text,
                c.CreatedAt));

            return Ok(response);
        }

        [Authorize]
        [HttpPost("{publicationId:guid}/comment")]
        public async Task<ActionResult> AddComment(
            Guid publicationId,
            [FromBody] CreateCommentRequest request,
            [FromServices] IValidator<CreateCommentRequest> validator)
        {
            var validationResults = await validator.ValidateAsync(request);
            if (!validationResults.IsValid)
                return BadRequest(validationResults.ToDictionary());

            var userId = Guid.Parse(User.FindFirst("id").Value);

            var comment = await _publicationsService.CreateComment(publicationId, userId, request.Text);

            var response = new GetCommentResponse(
                comment.Id,
                comment.PublicationId,
                comment.UserId,
                comment.Text,
                comment.CreatedAt);

            return Ok(response);
        }

        [Authorize]
        [HttpDelete("comment/{commentId:guid}")]
        public async Task<ActionResult> RemoveComment(Guid commentId)
        {
            var userId = Guid.Parse(User.FindFirst("id").Value);

            await _publicationsService.DeleteComment(userId, commentId);

            return NoContent();
        }

        [Authorize]
        [HttpGet("{publicationId:guid}/like")]
        public async Task<ActionResult> GetLike(Guid publicationId)
        {
            var userId = Guid.Parse(User.FindFirst("id").Value);
            bool likeStatus = await _publicationsService.GetLike(publicationId, userId);

            return Ok(likeStatus);
        }

        [Authorize]
        [HttpGet("{publicationId:guid}/likescount")]
        public async Task<ActionResult> GetLikesCount(Guid publicationId)
        {
            var likesCount = await _publicationsService.GetLikesCount(publicationId);

            return Ok(likesCount);
        }

        [Authorize]
        [HttpPost("{publicationId:guid}/like")]
        public async Task<ActionResult> Like(Guid publicationId)
        {
            var userId = Guid.Parse(User.FindFirst("id").Value);

            await _publicationsService.CreateLike(publicationId, userId);

            return Ok();
        }

        [Authorize]
        [HttpDelete("{publicationId:guid}/unlike")]
        public async Task<ActionResult> Unlike(Guid publicationId)
        {
            var userId = Guid.Parse(User.FindFirst("id").Value);

            await _publicationsService.DeleteLike(publicationId, userId);

            return Ok();
        }
    }
}
