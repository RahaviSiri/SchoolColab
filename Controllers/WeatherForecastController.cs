using Microsoft.AspNetCore.Mvc;

namespace SchoolColab.Controllers
{
    [ApiController]
    [Route("[controller]")]
	// It gets replaced with the name of your controller class, but without the “Controller” suffix. Here Controller name is WeatherForecastController so route is /WeatherForecast
	public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        [HttpGet(Name = "GetWeatherForecast")] // Name is optional; it allows you to reference this route elsewhere.
		public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
