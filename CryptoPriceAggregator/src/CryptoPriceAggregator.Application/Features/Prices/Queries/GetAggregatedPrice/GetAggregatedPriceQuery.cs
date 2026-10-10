
namespace CryptoPriceAggregator.Application.Features.Prices.Queries.GetAggregatedPrice;

public record GetAggregatedPriceQuery(
    string Instrument,
    DateTime TimestampUtc);