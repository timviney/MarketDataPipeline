using System.Text;
using MarketReplay.Infrastructure.Validation;
using Microsoft.IdentityModel.Tokens;

namespace MarketReplay.Api.Validation;

public static class JwtConfigurator
{
    public static void ConfigureJwtAuthentication(WebApplicationBuilder builder)
    {
        var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not found.");
        var jwtIssuer = builder.Configuration["Jwt:Authority"] ?? throw new InvalidOperationException("Jwt:Authority not found.");
        var jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience not found.");
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        
        builder.Services.AddSingleton(new TokenService(jwtIssuer, jwtAudience, signingKey));

        builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // Disabled for simplicity in Docker
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = jwtAudience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true, 
                    IssuerSigningKey = signingKey
                };
            });
    }
}