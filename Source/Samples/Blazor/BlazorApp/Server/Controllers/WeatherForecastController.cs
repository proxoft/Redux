using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Proxoft.Redux.BlazorApp.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Proxoft.Redux.BlazorApp.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController(ILogger<WeatherForecastController> logger) : ControllerBase
{
    private static readonly string[] _summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    private readonly ILogger<WeatherForecastController> _logger = logger;

    [HttpGet]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        await Task.Delay(3000);

        var rng = new Random();
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = rng.Next(-20, 55),
            Summary = _summaries[rng.Next(_summaries.Length)]
        })
        .ToArray();
    }
}
