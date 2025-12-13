using System.Threading.Channels;
using MarketReplay.Api.Background;
using MarketReplay.Api.Endpoints;
using MarketReplay.Core.Domain.Interfaces;
using MarketReplay.Core.Services.Pipeline;
using MarketReplay.Core.Services.Pipeline.Processors;
using MarketReplay.Core.Services.Replay;
using MarketReplay.Infrastructure.Data;
using MarketReplay.Infrastructure.State;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    // Obviously unsafe but fine for a dummy project
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

builder.Services.AddSingleton<IReplayEngine, ReplayEngine>();
builder.Services.AddSingleton<IMarketStateStore, InMemoryMarketStateStore>();
builder.Services.AddSingleton<IDataDirectory, DataDirectory>();
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

app.UseCors("AllowAll");

var useSwagger = true; // usually "app.Environment.IsDevelopment();" but we want swagger on permanently
if (useSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection(); // Disabled for simplicity in Docker

app.MapReplayEndpoints();
app.MapSymbolEndpoints();

app.Run();