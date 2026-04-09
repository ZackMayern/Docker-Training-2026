var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Mock-only mode: DB/Redis integrations are disabled.
// builder.Services.AddDbContext<WeatherDbContext>(options => options.UseNpgsql(databaseConnection));
// builder.Services.AddStackExchangeRedisCache(options => options.Configuration = redisConnection);
// builder.Services.AddScoped<WeatherService>();

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

app.MapControllers();  // <-- wires up /api/weather

app.Run();