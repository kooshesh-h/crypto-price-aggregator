using CryptoPriceAggregator.Application.Features.Prices.Queries.GetAggregatedPrice;

namespace CryptoPriceAggregator.Application.Features.Prices.Queries.GetPriceHistory;

public record GetPriceHistoryItemResult(
    DateTime TimestampUtc,
    double Price,
    IReadOnlyList<PriceSourceResult> SourcePrices);

public record GetPriceHistoryResult(
    string Instrument,
    DateTime FromUtc,
    DateTime ToUtc,
    IReadOnlyList<GetPriceHistoryItemResult> Items);