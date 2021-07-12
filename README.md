# Redux

```csharp
builder.Services
  .UseRedux<ApplicationState>(ServiceLifetime.Scoped)
  .UseDefaultDispatcher()
  .UseReducer<ApplicationReducer>()
  .UseDefaultStateStream()
  .AddEffects(Assembly.GetExecutingAssembly())
  .Register()
  ;
```
