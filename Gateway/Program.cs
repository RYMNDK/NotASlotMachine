using Gateway.Services;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// register my own services
builder.Services.AddSingleton<IConnectionClient, ConnectionClient>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/Spin", async (ILogger<Program> logger, IConnectionClient _client, BetDTO playerBet) =>
{
    try
    {
        var response = await _client.getClient().PostAsJsonAsync("http://localhost:8081/Spin", playerBet);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<WinDTO>();
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        logger.LogError("Error calling Spin" + ex.ToString());
        return Results.Problem();
    }
})
.WithName("Spin")
.WithOpenApi();

app.MapPut("/UpdateBalance", async (ILogger<Program> logger, IConnectionClient _client, PlayerBalanceDTO playerBalance) =>
{
    try
    {
        var response = await _client.getClient().PutAsJsonAsync("http://localhost:8082/Balance", playerBalance);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<PlayerBalanceDTO>();
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        logger.LogError("Error calling UpdateBalance" + ex.ToString());
        return Results.Problem();
    }
})
.WithName("UpdateBalance")
.WithOpenApi();


app.MapPut("/boxConfig", async (ILogger<Program> logger, IConnectionClient _client, boxMachineDTO newConfiguration) =>
{
    try
    {
        var response = await _client.getClient().PutAsJsonAsync("http://localhost:8081/boxConfig", newConfiguration);
        response.EnsureSuccessStatusCode();

        return Results.Ok();
    }
    catch (Exception ex)
    {
        logger.LogError("Error calling boxConfig" + ex.ToString());
        return Results.Problem();
    }
})

.WithName("boxConfig")
.WithOpenApi();

app.Run();

// dto copied from each service
internal record BetDTO(int playerId, long bet);
internal record WinDTO(int playerId, long win, long balance, String resultCSV);
internal record PlayerBalanceDTO(int playerId, long balance);
internal record boxMachineDTO(int maxX, int maxY);
