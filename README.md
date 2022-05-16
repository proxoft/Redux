# Redux
Predictable state container for .NET C#. Inspired by ngrx library. This library is tightly connected to System.Reactive library.

## Quick-Start

#### State
The state is an object containing all the application data. It may be of any type (class, record, struct, primitive). The important part about it is that the state should be immutable (or it must be used as it was immutable).
That means: any time a state needs to be changed a new instance with modified data must be created.

``` C#
public record CounterState
{
	public int Value { get; init; }
}
```


#### Actions
*Actions* are commands or events containing information (name of the action and optionally other data in the action) sent from your application to your *store*. 
They only need to implement the markup interface Redux.IAction.

```C#
public class IncreaseCounterAction : IAction
{
	public IncreaseCounterAction(int byValue)
	{
		this.ByValue = byValue;
	}

	public int ByValue { get; }
}
    
public class ResetCounter : IAction { }

```

#### Reducers

A *reducer* is a [pure function](https://en.wikipedia.org/wiki/Pure_function) with ((TState)state, (IAction)action) => (TState)state signature.
The reducer transform current state into the next state according to action by creating a new instance with modified data.
The *pure function* requirement implies that the reducer cannot habe any side-effect (e.g. persisting state, fetching data from another service).
The reason is that the reducer must be predictable: it must always behave the same way and its behavior (result) depends only on input arguments.

```C#
public static class CounterReducer
{
	public static CounterState Reduce(CounterState state, IAction action)
	{
		return action switch
		{
			IncreaseCounterAction ia => state with { Value = state.Value + ia.ByValue },
			ResetCounterAction _ => new (),
			_ => state
		};
	}
}
```

#### Action Dispatcher and IObservable\<TState>

An *action dispatcher* is a service responsible for delivering actions from *UI* to the *Store*.
An IObservable\<ApplicationState> is a service responsible for pushing the state updates from *Store* to *UI*.


```C#
@page "/counter"

<h1>Counter</h1>

<p>Current count: @_state.Value</p>

<button class="btn btn-primary" @onclick="this.OnClick">Click me</button>
<button class="btn btn-secondary" @onclick="this.OnReset">Reset</button>

@code {
	private IDisposable _subscription;
	private CounterState _state = new ();
	
	[Inject]
	public IObservable<ApplicationState> AppStream { get; set; }

	[Inject]
	public IActionDispatcher Dispatcher { get; set; };

	private void OnClick()
	{
		this.Dispatcher.Dispatch(new IncreaseCounterAction(3));
	}

	private void OnReset()
	{
		this.Dispatcher.Dispatch(new ResetCounterAction());
	}

	protected override void OnInitialized()
	{
		base.OnInitialized();

		_subscription = this.AppStream
			.Select(a => a.Counter)
			.DistinctUntilChanged()
			.Subscribe(c => _state = c)
		);
	}
}
```

#### Effect
The Effect is a construct for all side-effects e.g. fetching data from REST, saving data to DB, execution of asynchronous tasks, etc.
Effect may observe actions, state or both and execute corresponding actions. It also may dispatch new actions.


```C#
public class ForecastEffect : BaseApplicationEffect
{
	private readonly HttpClient _httpClient;

	public ForecastEffect(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

    // effect is automatically subscribed and dispatches the resulting action
	private IObservable<IAction> FetchDataEffect => this.ActionStream
		.OfType<FetchWeatherForcastDataAction>()
		.SelectAsync(action => this.FetchData(action))
		.Select(data => new SetWeatherForecastAction(data));

	private async Task<WeatherForecast[]> FetchData(FetchWeatherForcastDataAction action)
	{
		try
		{
			var data = await _httpClient.GetFromJsonAsync<WeatherForecast[]>("WeatherForecast")
				?? Array.Empty<WeatherForecast>();

			return data;
		}
		catch
		{
			return Array.Empty<WeatherForecast>();
		}
	}
}
```

#### Store

The Store\<TState> wires it all together. It
* Holds application state of type TState.
* Executes reducers any time an action is dispatched via ActionDispatcher.
* Publishes update state
* Publishes actions and updated states to the effect

Store needs to be initialized with an initial state before usage

``` C#
var store = host.Services.GetRequiredService<Store<ApplicationState>>();
store.Initialize(() => ApplicationState.Init);
```

#### Builder
To configure the Store use the package Proxoft.Redux.Host

```csharp
IServiceCollection services;

services.AddRedux<ApplicationState>(builder => {
	builder.UseReducer<ApplicationStateReducer>()
		.AddEffects(Assembly.GetExecutingAssembly());
});
```
