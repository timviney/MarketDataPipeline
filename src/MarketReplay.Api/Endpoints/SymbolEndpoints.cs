using MarketReplay.Core.Domain.Interfaces;

namespace MarketReplay.Api.Endpoints;

public static class SymbolEndpoints
{
    public static IEndpointRouteBuilder MapSymbolEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/symbols");

        group.MapGet("/", (IMarketStateStore store) =>
            {
                var symbols = store.GetSymbols();
                return Results.Ok(new { Symbols = symbols });
            })
            .WithName("Get All Symbols")
            .WithTags("Symbols");

        group.MapGet("/{symbol}/latest", (string symbol, IMarketStateStore store) =>
            {
                var tick = store.GetLatest(symbol);
                return tick is null ? Results.NotFound() : Results.Ok(tick);
            })
            .WithName("Get Latest Tick")
            .WithTags("Symbols");

        // group.MapGet("/{symbol}/summary", (string symbol, IMarketStateStore store) =>
        //     {
        //         var summary = store.GetSummary(symbol);
        //         return summary is null ? Results.NotFound() : Results.Ok(summary);
        //     })
        //     .WithName("Get Symbol Summary")
        //     .WithTags("Symbols");

        // group.MapGet("/{symbol}/anomalies", (string symbol, IMarketStateStore store) =>
        //     {
        //         var anomalies = store.GetAnomalies(symbol);
        //         return Results.Ok(anomalies);
        //     })
        //     .WithName("Get Symbol Anomalies")
        //     .WithTags("Symbols");

        return routes;
    }
}