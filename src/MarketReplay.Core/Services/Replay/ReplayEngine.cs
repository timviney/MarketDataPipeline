using MarketReplay.Core.Domain.Interfaces;

namespace MarketReplay.Core.Services.Replay;

public class ReplayEngine(IMarketDataProvider dataProvider, IEventPipeline pipeline) : IReplayEngine
{
    private CancellationTokenSource? _stoppingTokenSource;
    private TaskCompletionSource<bool>? _pauseSignal;
    
    private bool _isRunning = false;
    private bool _isPaused = false;
    
    public async Task StartAsync(int speed = 1)
    {
        _stoppingTokenSource = new();
        _pauseSignal = new(TaskCreationOptions.RunContinuationsAsynchronously);
        _isRunning = true;

        await Task.Run(async () =>
        {
            var data = await dataProvider.LoadData();

            foreach (var tick in data.TakeWhile(tick => !_stoppingTokenSource.Token.IsCancellationRequested))
            {
                await pipeline.PublishAsync(tick);

                await HandlePause();

                await Task.Delay(1000 / speed, _stoppingTokenSource.Token);
            }
        });
    }

    public async Task PauseAsync()
    {
        if (_pauseSignal != null && !_isPaused)
        {
            _isPaused = true;
            _pauseSignal = new(TaskCreationOptions.RunContinuationsAsynchronously);
        }
        else if (_pauseSignal != null && _isPaused)
        {
            _isPaused = false;
            _pauseSignal.TrySetResult(true);
        }

        await Task.CompletedTask;
    }
    
    public async Task StopAsync()
    {
        _pauseSignal?.TrySetResult(true);

        if (_stoppingTokenSource != null)
        {
            await _stoppingTokenSource.CancelAsync();
        }
        _isRunning = false;

        await Task.CompletedTask;
    }

    public string GetStatus()
    {
        if (_isRunning)
            return _isPaused ? "Paused" : "Running";

        return "Stopped";
    }

    private async Task HandlePause()
    {
        while (_isPaused && !_stoppingTokenSource!.Token.IsCancellationRequested)
        {
            await _pauseSignal!.Task;
        }
    }
}