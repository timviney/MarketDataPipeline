using System.Threading.Channels;
using MarketReplay.Api.Background;
using MarketReplay.Api.Endpoints;
using MarketReplay.Core.Domain.Interfaces;
using MarketReplay.Core.Domain.Model;
using MarketReplay.Core.Services.Pipeline;
using MarketReplay.Core.Services.Pipeline.Processors;
using MarketReplay.Core.Services.Replay;
using MarketReplay.Infrastructure.Data;
using MarketReplay.Infrastructure.State;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

builder.Services.AddSingleton<IReplayEngine, ReplayEngine>();
builder.Services.AddSingleton<IMarketStateStore, InMemoryMarketStateStore>();
builder.Services.AddSingleton<IDataDirectory, ContainerDataDirectory>();
builder.Services.AddSingleton<IMarketDataProvider, CsvMarketDataProvider>();
builder.Services.AddSingleton<IEventPipeline, EventPipeline>();
builder.Services.AddSingleton(Channel.CreateUnbounded<EngineCommand>());
builder.Services.AddSingleton(new ReplayState());
builder.Services.AddHostedService<ReplayWorker>();
builder.Services.AddSingleton<IEventProcessor[]>(sp =>
[
    new StateStoreProcessor(sp.GetRequiredService<IMarketStateStore>()),
    new CalculationProcessor(sp.GetRequiredService<IMarketStateStore>())
]);

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