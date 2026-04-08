var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();  // <-- registers controller routing

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();  // <-- wires up /api/weather

app.Run();