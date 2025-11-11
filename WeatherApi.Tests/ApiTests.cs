using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using WeatherForecast;


namespace WeatherApi.Tests;


public class ApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;


    public ApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }


    [Fact]
    public async Task Get_WeatherForecast_Returns_200_And_Five_Items()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false // we want to see raw status codes
        });


        var response = await client.GetAsync("/weatherforecast");
        response.StatusCode.Should().Be(HttpStatusCode.OK);


        var payload = await response.Content.ReadFromJsonAsync<WeatherForecast[]>();
        payload.Should().NotBeNull();
        payload!.Length.Should().Be(5);
    }


    [Fact]
    public async Task Get_WeatherForecast_Has_Expected_Shape_And_Ranges()
    {
        var client = _factory.CreateClient();
        var payload = await client.GetFromJsonAsync<WeatherForecast[]>("/weatherforecast");
        payload.Should().NotBeNull();
        var items = payload!;
        
        var now = DateOnly.FromDateTime(DateTime.Now);
        items.Select(x => x.Date).Should().BeInAscendingOrder();
        items.Select(x => (x.Date.DayNumber - now.DayNumber)).Should().OnlyContain(d => d >= 1 && d <= 6);
        
        items.Select(x => x.TemperatureC).Should().OnlyContain(c => c >= -20 && c <= 54);
        
        items.Select(x => x.Summary).Should().OnlyContain(s => !string.IsNullOrWhiteSpace(s));
        
        foreach (var x in items)
        {
            var expectedF = 32 + (int)Math.Floor(x.TemperatureC / 0.5556);
            x.TemperatureF.Should().Be(expectedF);
        }
    }


    private record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF { get; init; }
    }
}