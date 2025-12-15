using System.Threading.Channels;
using MarketReplay.Core.Domain.Interfaces;

namespace MarketReplay.Api.Background;

public class ReplayWorker(IReplayEngine engine, Channel<EngineCommand> channel, ReplayState state) : BackgroundService
{
    private const int DefaultSpeed = 1;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var commandReader = channel.Reader;
        
        await state.UpdateState(isRunning: false, isPaused: false, speed: DefaultSpeed);

        while (!stoppingToken.IsCancellationRequested)
        {
            while (commandReader.TryRead(out var command))
            {
                switch (command)
                {
                    case StartEngineCommand:
                        var wasRunning = state.IsRunning; // only restart if already stopped
                        await state.UpdateState(isRunning: true, isPaused: false);
                        if (!wasRunning) await engine.StartAsync(); // load data
                        break;

                    case PauseEngineCommand:
                        await state.UpdateState(isPaused: !state.IsPaused);
                        break;

                    case StopEngineCommand:
                        await state.UpdateState(isRunning: false, isPaused: false);
                        break;
                    
                    case AdjustSpeedEngineCommand adjustSpeedCommand:
                        await state.UpdateState(speed: adjustSpeedCommand.Speed);
                        break;
                }
            }

            if (state is { IsRunning: true, IsPaused: false })
            {
                var hasMore = await engine.StepAsync();
                if (!hasMore) await state.UpdateState(isRunning: false, isPaused: false);
                
                if (state.Speed > 0) await Task.Delay(1000 / state.Speed, stoppingToken);
            }
            else
            {
                await commandReader.WaitToReadAsync(stoppingToken);
            }
        }
    }
}