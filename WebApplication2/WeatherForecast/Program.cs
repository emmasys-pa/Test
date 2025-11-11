using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
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
        var northpolePassword = "password";
        var northpoleHashed = SHA256.HashData( System.Text.Encoding.UTF8.GetBytes(northpolePassword));
        forecast.First().GetType().GetProperty("Password")?.SetValue(forecast.First(), northpolePassword);
        forecast.First().GetType().GetProperty("HashedPassword")?.SetValue(forecast.First(), northpoleHashed);
        return forecast;
    })
    .WithName("GetWeatherForecast");

app.Run();

namespace WeatherForecast
{
    public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)Math.Floor(TemperatureC / 0.5556);
        public string Password => "password";
        public byte[] HashedPassword => SHA256.HashData( System.Text.Encoding.UTF8.GetBytes(Password));
    }

    public partial class Program { }
}