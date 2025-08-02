using System.Net;
using System.Text.Json;
using iBartender.Core.Exceptions;


namespace iBartender.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = HttpStatusCode.InternalServerError;
            var message = "Internal Server Error";

            switch (exception)
            {
                case NotFoundException notFoundEx:
                    statusCode = HttpStatusCode.NotFound;
                    message = notFoundEx.Message;
                    break;
            
                case AlreadyExistsException alreadyExistsEx:
                    statusCode = HttpStatusCode.Conflict;
                    message = alreadyExistsEx.Message;
                    break;
            
                case InvalidCredentialsException invalidCredentialsEx:
                    statusCode = HttpStatusCode.Unauthorized; /////////////
                    message = invalidCredentialsEx.Message;
                    break;

                case InvalidFileException invalidFileEx:
                    statusCode = HttpStatusCode.BadRequest;
                    message = invalidFileEx.Message;
                    break;

                case InvalidPasswordException invalidPasswordEx:
                    statusCode = HttpStatusCode.BadRequest;
                    message = invalidPasswordEx.Message;
                    break;

                case InvalidOperationException invalidOperationEx:
                    statusCode = HttpStatusCode.BadRequest;
                    message = invalidOperationEx.Message;
                    break;

                case UnauthorizedAccessException unauthorizedAccessEx:
                    statusCode = HttpStatusCode.Forbidden;
                    message = unauthorizedAccessEx.Message;
                    break;
            
                default:
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = message
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
        }
    }
}
