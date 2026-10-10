namespace CryptoPriceAggregator.Application.Features.Prices.Queries.GetPriceHistory;

public record GetPriceHistoryQuery(
    string Instrument,
    DateTime FromUtc,
    DateTime ToUtc);