using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);
var serviceVersion = typeof(Program).Assembly.GetName().Version?.ToString() ?? "1.0.0";

builder.Services.AddControllers();
builder.Services.AddHealthChecks();
// Mock-only mode: DB/Redis integrations are disabled.
// builder.Services.AddDbContext<WeatherDbContext>(options => options.UseNpgsql(databaseConnection));
// builder.Services.AddStackExchangeRedisCache(options => options.Configuration = redisConnection);
// builder.Services.AddScoped<WeatherService>();
builder.Services
	.AddOpenTelemetry()
	.WithMetrics(metrics =>
	{
		metrics
			.SetResourceBuilder(ResourceBuilder.CreateDefault()
				.AddService("weather-app", serviceVersion: serviceVersion))
			.AddAspNetCoreInstrumentation()
			.AddRuntimeInstrumentation()
			.AddMeter("Weather.App")
			.AddPrometheusExporter();
	});

var app = builder.Build();

// Seeding is intentionally disabled in mock-only mode.
// using (var scope = app.Services.CreateScope())
// {
// 	var dbContext = scope.ServiceProvider.GetRequiredService<WeatherDbContext>();
// 	await dbContext.Database.EnsureCreatedAsync();
// 	await WeatherSeed.SeedAsync(dbContext);
// }

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapHealthChecks("/health");
app.MapPrometheusScrapingEndpoint("/metrics");
app.MapControllers();

app.Run();