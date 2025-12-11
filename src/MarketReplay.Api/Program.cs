using MarketReplay.Api.Endpoints;
using MarketReplay.Core.Domain.Interfaces;
using MarketReplay.Core.Domain.Services.Replay;
using MarketReplay.Core.Domain.Services.State;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

builder.Services.AddSingleton<IReplayEngine, ReplayEngine>();
builder.Services.AddSingleton<IMarketStateStore, InMemoryMarketStateStore>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapReplayEndpoints();
app.MapSymbolEndpoints();

app.Run();