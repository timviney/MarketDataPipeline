namespace MarketReplay.Api.Background;

public abstract class EngineCommand
{
    public ReplayCommand Command { get; set; }
}

public class StartEngineCommand : EngineCommand
{
    public int Speed { get; set; }
    public StartEngineCommand(int speed)
    {
        Command = ReplayCommand.Start;
        Speed = speed;
    }
}

public class PauseEngineCommand : EngineCommand
{
    public PauseEngineCommand()
    {
        Command = ReplayCommand.Pause;
    }
}

public class StopEngineCommand : EngineCommand
{
    public StopEngineCommand()
    {
        Command = ReplayCommand.Stop;
    }
}