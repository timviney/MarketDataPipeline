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
                
                // Pretending here that this is some external service like Auth0 that actually authenticates the user
                // before returning a valid token to use for the APIs/SignalR

                return Results.Ok(new { Token = token });
            })
            .WithName("Login")
            .WithTags("Login")
            .AllowAnonymous(); // gotta allow anonymous entry to login
        
        return routes;
    }
}