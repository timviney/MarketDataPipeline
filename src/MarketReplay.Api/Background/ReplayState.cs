using System.Threading.Channels;

namespace MarketReplay.Api.Background;

// Singleton - source of truth
public class ReplayState(Channel<StateUpdate> channel)
{
    private readonly object _lock = new();

    public bool IsRunning { get; private set; }
    public bool IsPaused { get; private set; }
    public int Speed { get; private set; }
    
    public string Status => IsRunning ? (IsPaused ? "Paused" : "Running") : "Stopped";
    
    public async Task UpdateState(bool? isRunning = null, bool? isPaused = null, int? speed = null)
    {
        lock (_lock)
        {
            IsRunning = isRunning ?? IsRunning;
            IsPaused = isPaused ?? IsPaused;
            Speed = speed ?? Speed;
        }

        await channel.Writer.WriteAsync(new StateUpdate(this));
    }
}