using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using iBartender.Persistence;

namespace iBartender.API.Extensions
{
    public static class BuilderExtensions
    {
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidateAudience = true,
                        ValidAudience = configuration["Jwt:Audience"],
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            context.Token = context.Request.Cookies["pechenye"];
                            return Task.CompletedTask;
                        },

                        OnTokenValidated = async context =>
                        {
                            var dbContext = context.HttpContext.RequestServices
                                .GetRequiredService<BartenderDbContext>();

                            var userId = Guid.Parse(context.Principal.FindFirst("id").Value);
                            var user = await dbContext.Users.FindAsync(userId);
                            var tokenId = Guid.Parse(context.Principal.FindFirst("tokenId").Value);

                            if (user == null || user.TokenId != tokenId)
                                context.Fail("Invalid token version");
                        }
                    };
                });

            return services;
        }
    }
}
