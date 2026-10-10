using CryptoPriceAggregator.Domain.Entities;

namespace CryptoPriceAggregator.Application.Abstractions.Persistence;

public interface IPriceRepository
{
    Task<AggregatedPrice?> GetByInstrumentAndTimestampAsync(
        string instrument,
        DateTime timestampUtc,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AggregatedPrice>> GetByInstrumentAndRangeAsync(
        string instrument,
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        AggregatedPrice aggregatedPrice,
        CancellationToken cancellationToken = default);
}
