using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting; 

namespace RateLimitingAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [DisableRateLimiting]
    public class FixedWindowController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpGet]
        [EnableRateLimiting("FixedWindow")]
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

        [HttpGet]
        [Route("/GetById")]
       
        public WeatherForecast GetById(int i)
        {
            return new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(i)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[i]
            };
        }
    }
}