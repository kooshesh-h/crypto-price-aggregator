using CryptoPriceAggregator.Domain.Entities;

namespace CryptoPriceAggregator.Application.Abstractions.Pricing;

public interface IPriceSource
{
    string Name { get; }

    Task<SourcePrice> GetPriceAsync(
        string instrument,
        DateTime timestampUtc,
        CancellationToken cancellationToken = default);
}