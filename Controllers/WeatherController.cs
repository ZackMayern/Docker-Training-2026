using Microsoft.AspNetCore.Mvc;

namespace Docker_Training_2026.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private static readonly Dictionary<string, WeatherData> _db = new()
    {
        ["new york"]  = new("New York, US",  18, 16, 21, 14, 62, "14 km/h", "Partly cloudy",  "cloudy", new[]{ new Forecast("Tue",22,15,"sunny"), new("Wed",19,13,"rainy"), new("Thu",16,11,"rainy"), new("Fri",20,13,"cloudy"), new("Sat",24,16,"sunny") }),
        ["london"]    = new("London, UK",    12, 10, 14,  9, 75, "18 km/h", "Overcast",        "cloudy", new[]{ new Forecast("Tue",13, 8,"rainy"), new("Wed",11, 7,"rainy"), new("Thu",14, 9,"cloudy"), new("Fri",15,10,"cloudy"), new("Sat",16,11,"sunny") }),
        ["tokyo"]     = new("Tokyo, JP",     22, 21, 25, 18, 58, "10 km/h", "Clear skies",     "sunny",  new[]{ new Forecast("Tue",24,19,"sunny"), new("Wed",23,17,"cloudy"), new("Thu",20,16,"rainy"), new("Fri",22,17,"cloudy"), new("Sat",26,20,"sunny") }),
        ["mumbai"]    = new("Mumbai, IN",    34, 38, 36, 29, 82, "22 km/h", "Hot and humid",   "sunny",  new[]{ new Forecast("Tue",35,28,"sunny"), new("Wed",33,27,"cloudy"), new("Thu",31,26,"rainy"), new("Fri",30,26,"rainy"), new("Sat",32,27,"cloudy") }),
        ["sydney"]    = new("Sydney, AU",    20, 19, 23, 15, 55, "16 km/h", "Sunny intervals", "sunny",  new[]{ new Forecast("Tue",24,16,"sunny"), new("Wed",22,15,"sunny"), new("Thu",19,13,"cloudy"), new("Fri",18,12,"rainy"), new("Sat",21,14,"cloudy") }),
        ["paris"]     = new("Paris, FR",     15, 13, 17, 11, 68, "12 km/h", "Light rain",      "rainy",  new[]{ new Forecast("Tue",16,10,"rainy"), new("Wed",14, 9,"rainy"), new("Thu",17,11,"cloudy"), new("Fri",19,13,"sunny"), new("Sat",20,14,"sunny") }),
        ["dubai"]     = new("Dubai, AE",     38, 41, 40, 31, 45,  "8 km/h", "Clear and sunny", "sunny",  new[]{ new Forecast("Tue",41,32,"sunny"), new("Wed",39,31,"sunny"), new("Thu",38,30,"sunny"), new("Fri",37,30,"sunny"), new("Sat",39,31,"sunny") }),
        ["bengaluru"] = new("Bengaluru, IN", 27, 28, 30, 22, 60, "11 km/h", "Partly cloudy",   "cloudy", new[]{ new Forecast("Tue",29,21,"sunny"), new("Wed",28,21,"cloudy"), new("Thu",26,20,"rainy"), new("Fri",25,19,"rainy"), new("Sat",27,21,"cloudy") }),
        ["bangalore"] = new("Bengaluru, IN", 27, 28, 30, 22, 60, "11 km/h", "Partly cloudy",   "cloudy", new[]{ new Forecast("Tue",29,21,"sunny"), new("Wed",28,21,"cloudy"), new("Thu",26,20,"rainy"), new("Fri",25,19,"rainy"), new("Sat",27,21,"cloudy") }),
        ["new delhi"] = new("New Delhi, IN", 35, 39, 38, 28, 40, "15 km/h", "Hazy sunshine",   "sunny",  new[]{ new Forecast("Tue",37,29,"sunny"), new("Wed",36,28,"sunny"), new("Thu",34,27,"cloudy"), new("Fri",33,26,"cloudy"), new("Sat",35,27,"sunny") }),
    };

    [HttpGet]
    public IActionResult Get([FromQuery] string city)
    {
        if (string.IsNullOrWhiteSpace(city))
            return BadRequest(new { error = "City parameter is required." });

        var key = city.Trim().ToLower();

        if (!_db.TryGetValue(key, out var data))
            return NotFound(new { error = $"City '{city}' not found." });

        return Ok(data);
    }
}

public record Forecast(string Day, int Hi, int Lo, string Icon);

public record WeatherData(
    string City,
    int Temp,
    int Feels,
    int Hi,
    int Lo,
    int Humidity,
    string Wind,
    string Condition,
    string Icon,
    Forecast[] ForecastDays
);