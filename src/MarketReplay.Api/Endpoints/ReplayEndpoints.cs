using System.Threading.Channels;
using MarketReplay.Api.Background;
using MarketReplay.Core.Domain.Interfaces;

namespace MarketReplay.Api.Endpoints;

public static class ReplayEndpoints
{
    public static IEndpointRouteBuilder MapReplayEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/replay");

        group.MapPost("/start", async (ReplayState state, Channel<EngineCommand> ch, int speed) =>
            {
                await ch.Writer.WriteAsync(new StartEngineCommand(speed));
                return Results.Ok(new { State = state });
            })
            .WithName("Start Replay")
            .WithTags("Replay");

        group.MapPost("/pause", async (ReplayState state, Channel<EngineCommand> ch) =>
            {
                await ch.Writer.WriteAsync(new PauseEngineCommand());
                return Results.Ok(new { State = state });
            })
            .WithName("Pause Replay")
            .WithTags("Replay");

        group.MapPost("/stop", async (ReplayState state, Channel<EngineCommand> ch) =>
            {
                await ch.Writer.WriteAsync(new StopEngineCommand());

                return Results.Ok(new { State = state }); 
            })
            .WithName("Stop Replay")
            .WithTags("Replay");

        group.MapGet("/status", (ReplayState state) =>
            {
                return Results.Ok(new { State = state });
            })
            .WithName("Replay Status")
            .WithTags("Replay");

        return routes;
    }
}