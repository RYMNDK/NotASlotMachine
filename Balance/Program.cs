using Balance.Services;

var builder = WebApplication.CreateBuilder(args);

// Add CORS services allow from gateway and box
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalPorts", policy =>
    {
        policy
            .WithOrigins("https://localhost:8080", "https://localhost:7238",
                        "https://localhost:8081", "https://localhost:7101")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IBalanceManager, BalanceManager>();

var app = builder.Build();

app.UseCors("AllowLocalPorts");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();
