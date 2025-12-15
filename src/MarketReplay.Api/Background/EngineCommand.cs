namespace MarketReplay.Api.Background;

public abstract class EngineCommand
{
}

public class StartEngineCommand : EngineCommand
{
}

public class PauseEngineCommand : EngineCommand
{
}

public class StopEngineCommand : EngineCommand
{
}

public class AdjustSpeedEngineCommand(int speed) : EngineCommand
{
    public int Speed { get; set; } = speed;
}