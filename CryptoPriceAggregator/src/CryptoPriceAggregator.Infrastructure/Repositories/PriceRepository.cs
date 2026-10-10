using CryptoPriceAggregator.Application.Abstractions.Persistence;
using CryptoPriceAggregator.Domain.Entities;
using CryptoPriceAggregator.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CryptoPriceAggregator.Infrastructure.Repositories;

public class PriceRepository : IPriceRepository
{
    private readonly AppDbContext _dbContext;

    public PriceRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AggregatedPrice?> GetByInstrumentAndTimestampAsync(
        string instrument,
        DateTime timestampUtc,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.AggregatedPrices
            .AsNoTracking()
            .Include(x => x.SourcePrices)
            .SingleOrDefaultAsync(
                x => x.Instrument == instrument &&
                     x.TimestampUtc == timestampUtc,
                cancellationToken);
    }

    public async Task<IReadOnlyList<AggregatedPrice>> GetByInstrumentAndRangeAsync(
        string instrument,
        DateTime fromUtc,
        DateTime toUtc,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.AggregatedPrices
            .AsNoTracking()
            .Include(x => x.SourcePrices)
            .Where(x =>
                x.Instrument == instrument &&
                x.TimestampUtc >= fromUtc &&
                x.TimestampUtc <= toUtc)
            .OrderBy(x => x.TimestampUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        AggregatedPrice aggregatedPrice,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.AggregatedPrices.AddAsync(
            aggregatedPrice,
            cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}