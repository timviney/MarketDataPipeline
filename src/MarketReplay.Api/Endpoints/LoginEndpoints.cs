using MarketReplay.Infrastructure.Validation;

namespace MarketReplay.Api.Endpoints;

public static class LoginEndpoints
{
    public static IEndpointRouteBuilder MapLoginEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/login");

        group.MapGet("/", (TokenService tokenService) =>
            {
                var token = tokenService.CreateToken("12345", "admin");

                return Results.Ok(new { Token = token });
            })
            .WithName("Login")
            .WithTags("Login")
            .AllowAnonymous(); // gotta allow anonymous entry to login
        
        return routes;
    }
}