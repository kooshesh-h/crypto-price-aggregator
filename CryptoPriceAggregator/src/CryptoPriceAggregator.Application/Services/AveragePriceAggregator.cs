using CryptoPriceAggregator.Application.Abstractions.Pricing;

namespace CryptoPriceAggregator.Application.Services;

public class AveragePriceAggregator : IPriceAggregator
{
    public double Aggregate(IEnumerable<double> prices)
    {
        var priceList = prices.ToList();

        if (priceList.Count == 0)
            throw new InvalidOperationException(
                "At least one source price is required.");

        return priceList.Average();
    }
}