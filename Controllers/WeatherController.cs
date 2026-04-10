using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Mvc;
using Docker_Training_2026.Models;

namespace Docker_Training_2026.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController(ILogger<WeatherController> logger) : ControllerBase
{
    private static readonly Meter Meter = new("Weather.App");
    private static readonly Counter<long> LookupRequests = Meter.CreateCounter<long>("weather_lookup_requests");
    private static readonly Counter<long> LookupBadRequests = Meter.CreateCounter<long>("weather_lookup_bad_requests");
    private static readonly Counter<long> LookupNotFound = Meter.CreateCounter<long>("weather_lookup_not_found");
    private static readonly Histogram<double> LookupDurationMs = Meter.CreateHistogram<double>("weather_lookup_duration_ms");

    private static readonly Dictionary<string, WeatherData> MockWeatherData = new(StringComparer.OrdinalIgnoreCase)
    {
        ["mumbai"] = new WeatherData(
            "Mumbai, IN",
            34,
            38,
            36,
            29,
            82,
            "22 km/h",
            "Hot and humid",
            "sunny",
            [
                new Forecast("Tue", 35, 28, "sunny"),
                new Forecast("Wed", 33, 27, "cloudy"),
                new Forecast("Thu", 31, 26, "rainy"),
                new Forecast("Fri", 30, 26, "rainy"),
                new Forecast("Sat", 32, 27, "cloudy")
            ]),
        ["london"] = new WeatherData(
            "London, UK",
            12,
            10,
            14,
            9,
            75,
            "18 km/h",
            "Overcast",
            "cloudy",
            [
                new Forecast("Tue", 13, 8, "rainy"),
                new Forecast("Wed", 11, 7, "rainy"),
                new Forecast("Thu", 14, 9, "cloudy"),
                new Forecast("Fri", 15, 10, "cloudy"),
                new Forecast("Sat", 16, 11, "sunny")
            ]),
        ["new york"] = new WeatherData(
            "New York, US",
            18,
            16,
            21,
            14,
            62,
            "14 km/h",
            "Partly cloudy",
            "cloudy",
            [
                new Forecast("Tue", 22, 15, "sunny"),
                new Forecast("Wed", 19, 13, "rainy"),
                new Forecast("Thu", 16, 11, "rainy"),
                new Forecast("Fri", 20, 13, "cloudy"),
                new Forecast("Sat", 24, 16, "sunny")
            ]),
        ["tokyo"] = new WeatherData(
            "Tokyo, JP",
            22,
            21,
            25,
            18,
            58,
            "10 km/h",
            "Clear skies",
            "sunny",
            [
                new Forecast("Tue", 24, 19, "sunny"),
                new Forecast("Wed", 23, 17, "cloudy"),
                new Forecast("Thu", 20, 16, "rainy"),
                new Forecast("Fri", 22, 17, "cloudy"),
                new Forecast("Sat", 26, 20, "sunny")
            ]),
        ["sydney"] = new WeatherData(
            "Sydney, AU",
            20,
            19,
            23,
            15,
            55,
            "16 km/h",
            "Sunny intervals",
            "sunny",
            [
                new Forecast("Tue", 24, 16, "sunny"),
                new Forecast("Wed", 22, 15, "sunny"),
                new Forecast("Thu", 19, 13, "cloudy"),
                new Forecast("Fri", 18, 12, "rainy"),
                new Forecast("Sat", 21, 14, "cloudy")
            ]),
        ["paris"] = new WeatherData(
            "Paris, FR",
            15,
            13,
            17,
            11,
            68,
            "12 km/h",
            "Light rain",
            "rainy",
            [
                new Forecast("Tue", 16, 10, "rainy"),
                new Forecast("Wed", 14, 9, "rainy"),
                new Forecast("Thu", 17, 11, "cloudy"),
                new Forecast("Fri", 19, 13, "sunny"),
                new Forecast("Sat", 20, 14, "sunny")
            ]),
        ["dubai"] = new WeatherData(
            "Dubai, AE",
            38,
            41,
            40,
            31,
            45,
            "8 km/h",
            "Clear and sunny",
            "sunny",
            [
                new Forecast("Tue", 41, 32, "sunny"),
                new Forecast("Wed", 39, 31, "sunny"),
                new Forecast("Thu", 38, 30, "sunny"),
                new Forecast("Fri", 37, 30, "sunny"),
                new Forecast("Sat", 39, 31, "sunny")
            ]),
        ["bengaluru"] = new WeatherData(
            "Bengaluru, IN",
            27,
            28,
            30,
            22,
            60,
            "11 km/h",
            "Partly cloudy",
            "cloudy",
            [
                new Forecast("Tue", 29, 21, "sunny"),
                new Forecast("Wed", 28, 21, "cloudy"),
                new Forecast("Thu", 26, 20, "rainy"),
                new Forecast("Fri", 25, 19, "rainy"),
                new Forecast("Sat", 27, 21, "cloudy")
            ]),
        ["bangalore"] = new WeatherData(
            "Bengaluru, IN",
            27,
            28,
            30,
            22,
            60,
            "11 km/h",
            "Partly cloudy",
            "cloudy",
            [
                new Forecast("Tue", 29, 21, "sunny"),
                new Forecast("Wed", 28, 21, "cloudy"),
                new Forecast("Thu", 26, 20, "rainy"),
                new Forecast("Fri", 25, 19, "rainy"),
                new Forecast("Sat", 27, 21, "cloudy")
            ]),
        ["new delhi"] = new WeatherData(
            "New Delhi, IN",
            35,
            39,
            38,
            28,
            40,
            "15 km/h",
            "Hazy sunshine",
            "sunny",
            [
                new Forecast("Tue", 37, 29, "sunny"),
                new Forecast("Wed", 36, 28, "sunny"),
                new Forecast("Thu", 34, 27, "cloudy"),
                new Forecast("Fri", 33, 26, "cloudy"),
                new Forecast("Sat", 35, 27, "sunny")
            ])
    };

    [HttpGet]
    public IActionResult Get([FromQuery] string city)
    {
        var stopwatch = Stopwatch.StartNew();

        if (string.IsNullOrWhiteSpace(city))
        {
            LookupBadRequests.Add(1, new KeyValuePair<string, object?>("endpoint", "weather"));
            LookupDurationMs.Record(stopwatch.Elapsed.TotalMilliseconds, new KeyValuePair<string, object?>("outcome", "bad_request"));
            logger.LogWarning("Weather lookup rejected because city query parameter was missing.");
            return BadRequest(new { error = "City parameter is required." });
        }

        var key = city.Trim().ToLowerInvariant();
        LookupRequests.Add(1,
            new KeyValuePair<string, object?>("endpoint", "weather"),
            new KeyValuePair<string, object?>("city", key));

        var data = MockWeatherData.GetValueOrDefault(key);
        if (data is null)
        {
            LookupNotFound.Add(1,
                new KeyValuePair<string, object?>("endpoint", "weather"),
                new KeyValuePair<string, object?>("city", key));
            LookupDurationMs.Record(stopwatch.Elapsed.TotalMilliseconds,
                new KeyValuePair<string, object?>("outcome", "not_found"),
                new KeyValuePair<string, object?>("city", key));
            logger.LogInformation("Weather lookup returned no data for city {City}.", key);
            return NotFound(new { error = $"City '{city}' not found." });
        }

        LookupDurationMs.Record(stopwatch.Elapsed.TotalMilliseconds,
            new KeyValuePair<string, object?>("outcome", "success"),
            new KeyValuePair<string, object?>("city", key));
        logger.LogInformation("Weather lookup served for city {City}.", key);

        return Ok(data);
    }
}