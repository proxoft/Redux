namespace ConsoleApp.Application;

public record ApplicationState
{
    public int GuardedActionsCount { get; init; }

    public string Message { get; init; } = "";
}
