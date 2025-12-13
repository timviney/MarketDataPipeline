using System.Threading.Channels;
using MarketReplay.Core.Domain.Interfaces;

namespace MarketReplay.Api.Background;

public class ReplayWorker(IReplayEngine engine, Channel<EngineCommand> channel, ReplayState state) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var commandReader = channel.Reader;

        while (!stoppingToken.IsCancellationRequested)
        {
            while (commandReader.TryRead(out var command))
            {
                switch (command)
                {
                    case StartEngineCommand start:
                        state.UpdateState(isRunning: true, isPaused: false, speed: start.Speed);
                        await engine.StartAsync(); // load data
                        break;

                    case PauseEngineCommand:
                        state.UpdateState(isPaused: !state.IsPaused);
                        break;

                    case StopEngineCommand:
                        state.UpdateState(isRunning: false, isPaused: false);
                        break;
                }
            }

            if (state is { IsRunning: true, IsPaused: false })
            {
                var hasMore = await engine.StepAsync();
                if (!hasMore) state.UpdateState(isRunning: false, isPaused: false);
                
                if (state.Speed > 0) await Task.Delay(1000 / state.Speed, stoppingToken);
            }
            else
            {
                await commandReader.WaitToReadAsync(stoppingToken);
            }
        }
    }
}