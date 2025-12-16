using System.Text;
using System.Threading.Channels;
using MarketReplay.Api.Background;
using MarketReplay.Api.Endpoints;
using MarketReplay.Api.Hubs;
using MarketReplay.Api.Validation;
using MarketReplay.Core.Application;
using MarketReplay.Core.Domain.Interfaces;
using MarketReplay.Core.Services.Pipeline;
using MarketReplay.Core.Services.Pipeline.Processors;
using MarketReplay.Core.Services.Replay;
using MarketReplay.Infrastructure.Data;
using MarketReplay.Infrastructure.SignalR;
using MarketReplay.Infrastructure.State;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    // Make everything public but require auth on all endpoints
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .SetIsOriginAllowed(_ => true);
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
builder.Services.AddSingleton(Channel.CreateUnbounded<StateUpdate>());
builder.Services.AddSingleton<ITickCalculationPublisher, TickCalculationPublisher>();
builder.Services.AddSingleton<IReplayStatePublisher, ReplayStatePublisher>();
builder.Services.AddSingleton(sp => new ReplayState(sp.GetRequiredService<Channel<StateUpdate>>(), sp.GetRequiredService<IReplayStatePublisher>()));
builder.Services.AddHostedService<ReplayWorker>();
builder.Services.AddSingleton<IEventProcessor[]>(sp =>
[
    new StateStoreProcessor(sp.GetRequiredService<IMarketStateStore>()),
    new CalculationProcessor(sp.GetRequiredService<IMarketStateStore>(), sp.GetRequiredService<ITickCalculationPublisher>())
]);

JwtConfigurator.ConfigureJwtAuthentication(builder);

builder.Services.Configure<HubOptions>(options =>
{
    options.StatefulReconnectBufferSize = 1000;
});

builder.Services.AddSignalR();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your Bearer token from the login endpoint in the format: \"{your JWT token}\"",
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | 
                       Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
});

app.UseCors("AllowAll");
app.UseAuthentication(); 
app.UseAuthorization();

app.UseWebSockets();

var useSwagger = true; // usually "app.Environment.IsDevelopment();" but we want swagger on permanently
if (useSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection(); // Disabled for simplicity in Docker

app.MapReplayEndpoints();
app.MapSymbolEndpoints();
app.MapLoginEndpoints();

HubMappings.MapSignalRHubs(app);

app.Run();