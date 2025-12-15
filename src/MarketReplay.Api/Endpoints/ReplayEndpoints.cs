using System.Threading.Channels;
using MarketReplay.Api.Background;
using MarketReplay.Core.Domain.Interfaces;

namespace MarketReplay.Api.Endpoints;

public static class ReplayEndpoints
{
    public static IEndpointRouteBuilder MapReplayEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/replay");

        group.RequireAuthorization();

        group.MapPost("/start", async (ReplayState state, Channel<EngineCommand> engineCommandChannel, Channel<StateUpdate> stateUpdateChannel) =>
            {
                await engineCommandChannel.Writer.WriteAsync(new StartEngineCommand());
                await WaitForStateUpdateAsync(stateUpdateChannel);
                
                return Results.Ok(new { State = state });
            })
            .WithName("Start Replay")
            .WithTags("Replay");

        group.MapPost("/pause", async (ReplayState state, Channel<EngineCommand> engineCommandChannel, Channel<StateUpdate> stateUpdateChannel) =>
            {
                await engineCommandChannel.Writer.WriteAsync(new PauseEngineCommand());
                await WaitForStateUpdateAsync(stateUpdateChannel);
                
                return Results.Ok(new { State = state });
            })
            .WithName("Pause Replay")
            .WithTags("Replay");

        group.MapPost("/stop", async (ReplayState state, Channel<EngineCommand> engineCommandChannel, Channel<StateUpdate> stateUpdateChannel) =>
            {
                await engineCommandChannel.Writer.WriteAsync(new StopEngineCommand());
                await WaitForStateUpdateAsync(stateUpdateChannel);

                return Results.Ok(new { State = state }); 
            })
            .WithName("Stop Replay")
            .WithTags("Replay");

        group.MapPost("/adjustspeed", async (ReplayState state, Channel<EngineCommand> engineCommandChannel, Channel<StateUpdate> stateUpdateChannel, int speed) =>
            {
                await engineCommandChannel.Writer.WriteAsync(new AdjustSpeedEngineCommand(speed));
                await WaitForStateUpdateAsync(stateUpdateChannel);

                return Results.Ok(new { State = state }); 
            })
            .WithName("Adjust Replay Speed")
            .WithTags("Replay");

        group.MapGet("/status", (ReplayState state) =>
            {
                return Results.Ok(new { State = state });
            })
            .WithName("Replay Status")
            .WithTags("Replay");

        return routes;
    }

    private static async Task WaitForStateUpdateAsync(Channel<StateUpdate> stateUpdateChannel)
    {
        var maxWaitCancellation = new CancellationTokenSource(TimeSpan.FromSeconds(2));
        await stateUpdateChannel.Reader.WaitToReadAsync(maxWaitCancellation.Token); // wait for state to be updated
    }
}