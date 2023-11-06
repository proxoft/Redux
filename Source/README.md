# Redux for .NET
Predictable state management.
The Redux is composed of logical components
1. State - immutable state without any logic
2. Action - a command or even which triggers changes on state
3. Reducer - pure function which creates a new state based on action and current state
4. Effects - serves as a side effect (e.g. reading/writing to database, using REST, etc.)

All of these are orchestrated by Store

## Getting started

### Define components

1. Create 'State' 

```csharp
public record ApplicationState
{
    public string Message { get; init; } = "";
}
```

2. Create 'Actions'

```csharp
public class SetMessageAction : IAction
{
    public SetMessageAction(string message)
    {
        this.Message = message;
    }

    public string Message { get; }
}

public class TriggerAction : IAction
{
}

public class ResetMessageAction : IAction
{
}
```

3. Create 'Reducer'

```csharp
public static class ApplicationReducer
{
    public static ApplicationState Reduce(ApplicationState state, IAction action)
    {
        return action switch
        {
            SetMessageAction messageAction => state with { Message = messageAction.Message },
            ResetMessageAction => state with { Message = "" },
            _ => state
        };
    }
}
```

4. Create 'Effect'

```csharp
public class ApplicationEffect : Effect<ApplicationState>
{
    private IObservable<IAction> Effect => this.ActionStream
            .OfType<TriggerAction>()
            .Select(_ => new SetMessageAction("Triggered SetMessage"))
            ;
}
```

### Create Store
```csharp

Store store = StoreHelper.Create(ApplicationReducer.Reduce, effects: new ApplicationEffect());
store.Initialize(new ApplicationState());

store.Dispatchar.Dispatch(new TriggerAction());

```

### Use the store

Dispatch actions

```csharp

store.Dispatcher.Dispatch(new TriggerAction());
store.Dispatcher.Dispatch(new ResetMessageAction());

```

Note: it is possible to register and inject own implementation of IActionDispatcher using DI. (see proxoft.redux.hosting package)

Subscribe to state changes

```csharp

store.StateStream
    .Select(state => {
        Console.WriteLine(state.Message);
    });

```

## Registering services
use the proxoft.redux.hosting package

```csharp

services.AddRedux<ApplicationState>(
            builder =>
            {
                builder
                    .UseScheduler(Scheduler.Default)
                    .UseJournaler<ActionJournaler>()
                    .UseExceptionHandler<DefaultExceptionHandler>()
                    .UseReducer(ApplicationReduce.Reduce)
                    .AddEffects(typeof(ApplicationState).Assembly);
            });

```

## Other features
In some scenarios it might be useful to suppress actions that have been dispatched, for example repetitive dispatch of an action or concurrent state changes causing dispatched action to become invalid.

```csharp
public record ApplicationState
    {
        public bool ProcessRunning { get; init; }
        public string LastMessage { get; init; } = "";
}

    public class StartProcessAction : IAction
    {
    }

    public class StopProcessAction : IAction
    {
    }

    public class WarningAction(string message) : IAction
    {
        public string Message { get; } = message;
    }

    public static class ApplicationReducer
    {
        public static ApplicationState Reduce(ApplicationState state, IAction action)
        {
            return action switch
            {
                StartProcessAction => state with { ProcessRunning = true },
                StopProcessAction => state with { ProcessRunning = false },
                WarningAction a => state with { LastMessage = a.Message },
                _ => state
            };
        }
    }

    public class ExternProcessRunner
    {
        private bool _started;

        public void Start()
        {
            if (_started)
            {
                throw new Exception("cannot start");
            }

            _started = true;
        }

        public void Stop()
        {
            if (_started)
            {
                throw new Exception("cannot stop");
            }

            _started = false;
        }
    }

    public class ProcessEffect(ExternProcessRunner runner) : Effect<ApplicationState>
    {
        private readonly ExternProcessRunner _runner = runner;

        private IObservable<Unit> OnStart => this.ActionStream
                .OfType<StartProcessAction>()
                .Do(_ => _runner.Start())
                .SelectVoid()
                ;

        private IObservable<Unit> OnStop => this.ActionStream
                .OfType<StopProcessAction>()
                .Do(_ => _runner.Stop())
                .SelectVoid()
                ;
    }

    public static class ActionGuard
    {
        public static IAction Validate(IAction action, ApplicationState state)
        {
            return action switch
            {
                StartProcessAction when state.ProcessRunning => new WarningAction("process already started"),
                StopProcessAction when state.ProcessRunning == false => new WarningAction("process already stopped"),
                _ => action
            };
        }
    }

    // ...

    Store<ApplicationState> store = StoreHelper.Create<ApplicationState>(
        ApplicationReducer.Reduce,
        ActionGuard.Validate,
        effects: new ProcessEffect(new ExternProcessRunner())
    );

    store.Initialize(new ApplicationState());

    store.Dispatcher.Dispatch(new StartProcessAction());
    store.Dispatcher.Dispatch(new StartProcessAction()); // will be changed to WarningAction

    store.Dispatcher.Dispatch(new StopProcessAction());
    store.Dispatcher.Dispatch(new StopProcessAction());  // will be changed to WarningAction
```