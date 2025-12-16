using System.Threading.Channels;
using MarketReplay.Core.Domain.Interfaces;

namespace MarketReplay.Core.Application;

// Singleton - source of truth
public class ReplayState(Channel<StateUpdate> channel, IReplayStatePublisher statePublisher)
{
    private readonly object _lock = new();

    public bool IsRunning { get; private set; }
    public bool IsPaused { get; private set; }
    public int Speed { get; private set; }
    
    public string Status => IsRunning ? (IsPaused ? "Paused" : "Running") : "Stopped";
    
    public async Task UpdateState(bool? isRunning = null, bool? isPaused = null, int? speed = null)
    {
        bool wasRunning;
        lock (_lock)
        {
            wasRunning = IsRunning;
            IsRunning = isRunning ?? IsRunning;
            IsPaused = isPaused ?? IsPaused;
            Speed = speed ?? Speed;
        }

        await statePublisher.PublishAsync(this); // For SignalR
        await channel.Writer.WriteAsync(new StateUpdate(this)); // For APIs
        
        var hasBeenCleared = !wasRunning && IsRunning;
        if (hasBeenCleared) await statePublisher.PublishClearedMessage();
    }
}