# Redux for .NET
Predictable state container for .NET C#. Inspired by ngrx library. This library is tightly connected to System.Reactive library.

The Redux is composed of logical components
1. State - immutable state without any logic
2. Action - a command or even which triggers changes on state
3. Reducer - pure function which creates a new state based on action and current state
4. Effects - serves as a side effect (e.g. reading/writing to database, using REST, etc.)

All of these are orchestrated by Store

## Getting started

### Define components

#### State
The state is an object containing all the application data. It may be of any type (class, record, struct, primitive). The important part about it is that the state should be immutable (or it must be used as it was immutable).
That means: any time a state needs to be changed a new instance with modified data must be created.

```csharp
public record ApplicationState
{
    public string Message { get; init; } = "";
}
```

#### Actions
*Actions* are commands or events containing information (name of the action and optionally other data in the action) sent from your application to your *store*. 
They only need to implement the markup interface Redux.IAction.

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

#### Reducers
A *reducer* is a [pure function](https://en.wikipedia.org/wiki/Pure_function) with ((TState)state, (IAction)action) => (TState)state signature.
The reducer transform current state into the next state according to action by creating a new instance with modified data.
The *pure function* requirement implies that the reducer cannot habe any side-effect (e.g. persisting state, fetching data from another service).
The reason is that the reducer must be predictable: it must always behave the same way and its behavior (result) depends only on input arguments.

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

#### Effect
The Effect is a construct for all side-effects e.g. fetching data from REST, saving data to DB, execution of asynchronous tasks, etc.
Effect may observe actions, state or both and execute corresponding actions. It also may dispatch new actions.

```csharp
public class ApplicationEffect : Effect<ApplicationState>
{
    private IObservable<IAction> Effect => this.ActionStream
            .OfType<TriggerAction>()
            .Select(_ => new SetMessageAction("Triggered SetMessage"))
            ;
}
```


#### Store

The Store\<TState> wires it all together. It
* Holds application state of type TState.
* Executes reducers any time an action is dispatched via ActionDispatcher.
* Publishes update state
* Publishes actions and updated states to the effect

```csharp

Store store = StoreHelper.Create(ApplicationReducer.Reduce, effects: new ApplicationEffect());
store.Initialize(new ApplicationState());

store.Dispatchar.Dispatch(new TriggerAction());

```

#### Use the store

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

#### Builder
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