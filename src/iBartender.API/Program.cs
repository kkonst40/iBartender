using iBartender.API.Extensions;
using iBartender.API.Middleware;
using iBartender.Application.Services;
using iBartender.Application.Utils;
using iBartender.Persistence;
using iBartender.Persistence.Repositories;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;

namespace IBartender.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Default", policy =>
                {
                    policy
                    .WithOrigins("http://localhost:3000", "http://127.0.0.1:3000")
                    .AllowCredentials()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddJwtAuthentication(builder.Configuration);
            builder.Services.AddAuthorization();

            builder.Services.AddDbContext<BartenderDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("BartenderDb")));

            builder.Services.AddScoped<IUsersService, UsersService>();
            builder.Services.AddScoped<IPublicationsService, PublicationsService>();
            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
            builder.Services.AddScoped<IJwtProvider, JwtProvider>();
            builder.Services.AddScoped<IImageProcessor, ImageProcessor>();
            builder.Services.AddScoped<ICredentialsValidator, CredentialsValidator>();

            builder.Services.AddScoped<IUsersRepository, UsersRepository>();
            builder.Services.AddScoped<IPublicationsRepository, PublicationsRepository>();


            var app = builder.Build();

            app.UseStaticFiles();
            app.UseCookiePolicy(new CookiePolicyOptions
            {
                Secure = CookieSecurePolicy.Always,
                MinimumSameSitePolicy = SameSiteMode.None,
                HttpOnly = HttpOnlyPolicy.Always,
            });

            app.UseCors("Default");

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
