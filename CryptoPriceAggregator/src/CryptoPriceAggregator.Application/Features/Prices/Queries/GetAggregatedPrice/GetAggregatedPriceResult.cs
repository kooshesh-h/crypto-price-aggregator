namespace CryptoPriceAggregator.Application.Features.Prices.Queries.GetAggregatedPrice;

public record PriceSourceResult(
    string SourceName,
    double Price);

public record GetAggregatedPriceResult(
    string Instrument,
    DateTime TimestampUtc,
    double Price,
    bool IsFromCache,
    IReadOnlyList<PriceSourceResult> SourcePrices);