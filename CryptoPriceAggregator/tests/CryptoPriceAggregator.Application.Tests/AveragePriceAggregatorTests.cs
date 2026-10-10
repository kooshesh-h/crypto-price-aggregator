using CryptoPriceAggregator.Application.Services;
using CryptoPriceAggregator.Domain.Entities;
using Xunit;

namespace CryptoPriceAggregator.Application.Tests;

public class AveragePriceAggregatorTests
{
    [Fact]
    public void Aggregate_TwoSourcePrices_ReturnsTheirAverage()
    {
        var sourcePrices = new[]
        {
            new SourcePrice("Bitstamp", 100d),
            new SourcePrice("Bitfinex", 120d)
        };
        var aggregator = new AveragePriceAggregator();
        var aggregatedPrice = aggregator.Aggregate(sourcePrices.Select(source => source.Price));

        Assert.Equal(110d, aggregatedPrice);
    }
}
