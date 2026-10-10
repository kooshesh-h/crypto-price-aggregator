using CryptoPriceAggregator.Application.Abstractions.Persistence;
using CryptoPriceAggregator.Application.Abstractions.Pricing;
using CryptoPriceAggregator.Domain.Entities;

namespace CryptoPriceAggregator.Application.Features.Prices.Queries.GetAggregatedPrice;

public class GetAggregatedPriceQueryHandler
{
    private readonly IPriceRepository _priceRepository;
    private readonly IEnumerable<IPriceSource> _priceSources;
    private readonly IPriceAggregator _priceAggregator;

    public GetAggregatedPriceQueryHandler(
        IPriceRepository priceRepository,
        IEnumerable<IPriceSource> priceSources,
        IPriceAggregator priceAggregator)
    {
        _priceRepository = priceRepository;
        _priceSources = priceSources;
        _priceAggregator = priceAggregator;
    }

    public async Task<GetAggregatedPriceResult> HandleAsync(
        GetAggregatedPriceQuery query,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query.Instrument))
            throw new ArgumentException("Instrument is required.");

        if (query.TimestampUtc.Kind != DateTimeKind.Utc)
            throw new ArgumentException(
                "Timestamp must be specified in UTC.");

        if (query.TimestampUtc.Minute != 0 ||
            query.TimestampUtc.Second != 0 ||
            query.TimestampUtc.Millisecond != 0)
        {
            throw new ArgumentException(
                "Timestamp must have hour accuracy.");
        }

        var instrument = query.Instrument.Trim().ToUpperInvariant();

        var cachedPrice = await _priceRepository
            .GetByInstrumentAndTimestampAsync(
                instrument,
                query.TimestampUtc,
                cancellationToken);

        if (cachedPrice is not null)
            return MapToResult(cachedPrice, isFromCache: true);

        var sources = _priceSources.ToList();

        if (sources.Count == 0)
            throw new InvalidOperationException(
                "No price sources are configured.");

        var sourcePrices = await Task.WhenAll(
            sources.Select(source =>
                source.GetPriceAsync(
                    instrument,
                    query.TimestampUtc,
                    cancellationToken)));

        var aggregatedValue = _priceAggregator.Aggregate(
            sourcePrices.Select(x => x.Price));

        var aggregatedPrice = new AggregatedPrice(
            instrument,
            query.TimestampUtc,
            aggregatedValue,
            sourcePrices);

        await _priceRepository.AddAsync(
            aggregatedPrice,
            cancellationToken);

        return MapToResult(aggregatedPrice, isFromCache: false);
    }

    private static GetAggregatedPriceResult MapToResult(
        AggregatedPrice aggregatedPrice,
        bool isFromCache)
    {
        return new GetAggregatedPriceResult(
            aggregatedPrice.Instrument,
            DateTime.SpecifyKind(aggregatedPrice.TimestampUtc, DateTimeKind.Utc),
            aggregatedPrice.Price,
            isFromCache,
            aggregatedPrice.SourcePrices
                .Select(sourcePrice => new PriceSourceResult(
                    sourcePrice.SourceName,
                    sourcePrice.Price))
                .ToList());
    }
}