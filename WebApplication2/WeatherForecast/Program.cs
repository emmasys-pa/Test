var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast.WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");


// ✅ NY ROUTE for Nordpolen
app.MapGet("/weatherforecast/northpole", () =>
    {
        var summaries = new[]
        {
            "Extremely Freezing", "Arctic", "Frigid", "Icy", "Snowstorm", "Polar Night"
        };

        var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast.WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-50, -10), // kaldere temperaturer
                summaries[Random.Shared.Next(summaries.Length)]
            )).ToArray();

        return forecast;
    })
    .WithName("GetNorthPoleWeatherForecast");

app.Run();

namespace WeatherForecast
{
    public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)Math.Floor(TemperatureC / 0.5556);
    }

    public partial class Program { }
}