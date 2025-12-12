using MarketReplay.Core.Domain.Interfaces;

namespace MarketReplay.Api.Endpoints;

public static class ReplayEndpoints
{
    public static IEndpointRouteBuilder MapReplayEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/replay");

        group.MapPost("/start", async (IReplayEngine replay, int speed) =>
            {
                await replay.StartAsync(speed); //TODO shouldn't get stuck here
                return Results.Ok(new { Status = "Started", Speed = speed });
            })
            .WithName("Start Replay")
            .WithTags("Replay");

        group.MapPost("/pause", async (IReplayEngine replay) =>
            {
                await replay.PauseAsync();
                return Results.Ok(new { Status = "Paused" });
            })
            .WithName("Pause Replay")
            .WithTags("Replay");

        group.MapPost("/stop", async (IReplayEngine replay) =>
            {
                await replay.StopAsync(); //TODO should crash here
                return Results.Ok(new { Status = "Stopped" }); 
            })
            .WithName("Stop Replay")
            .WithTags("Replay");

        group.MapGet("/status", (IReplayEngine replay) =>
            {
                var status = replay.GetStatus();
                return Results.Ok(new { Status = status });
            })
            .WithName("Replay Status")
            .WithTags("Replay");

        return routes;
    }
}