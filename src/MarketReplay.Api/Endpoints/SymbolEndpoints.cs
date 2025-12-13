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

        group.MapGet("/{symbol}/history", (string symbol, IMarketStateStore store) =>
            {
                var history = store.GetHistory(symbol);
                return Results.Ok(history);
            })
            .WithName("Get Symbol History")
            .WithTags("Symbols");

        group.MapGet("/{symbol}/calculations", (string symbol, IMarketStateStore store) =>
            {
                var history = store.GetCalculationHistory(symbol);
                return Results.Ok(history);
            })
            .WithName("Get Symbol Calculations")
            .WithTags("Symbols");

        return routes;
    }
}