using CryptoPriceAggregator.Application.Abstractions.Persistence;
using CryptoPriceAggregator.Application.Features.Prices.Queries.GetAggregatedPrice;

namespace CryptoPriceAggregator.Application.Features.Prices.Queries.GetPriceHistory;

public class GetPriceHistoryQueryHandler
{
    private readonly IPriceRepository _priceRepository;

    public GetPriceHistoryQueryHandler(IPriceRepository priceRepository)
    {
        _priceRepository = priceRepository;
    }

    public async Task<GetPriceHistoryResult> HandleAsync(
        GetPriceHistoryQuery query,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query.Instrument))
        {
            throw new ArgumentException("Instrument is required.", nameof(query.Instrument));
        }

        if (query.FromUtc.Kind != DateTimeKind.Utc ||
            query.ToUtc.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("FromUtc and ToUtc must be in UTC.");
        }

        if (query.FromUtc > query.ToUtc)
        {
            throw new ArgumentException("FromUtc must be earlier than or equal to ToUtc.");
        }

        var instrument = query.Instrument.Trim().ToUpperInvariant();

        var prices = await _priceRepository.GetByInstrumentAndRangeAsync(
            instrument,
            query.FromUtc,
            query.ToUtc,
            cancellationToken);

        var items = prices
            .Select(price => new GetPriceHistoryItemResult(
                DateTime.SpecifyKind(price.TimestampUtc, DateTimeKind.Utc),
                price.Price,
                price.SourcePrices
                    .Select(source => new PriceSourceResult(
                        source.SourceName,
                        source.Price))
                    .ToList()))
            .ToList();

        return new GetPriceHistoryResult(
            instrument,
            query.FromUtc,
            query.ToUtc,
            items);
    }
}