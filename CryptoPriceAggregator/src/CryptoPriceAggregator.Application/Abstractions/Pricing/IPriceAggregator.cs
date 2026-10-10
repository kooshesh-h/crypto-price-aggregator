namespace CryptoPriceAggregator.Application.Abstractions.Pricing;

public interface IPriceAggregator
{
    double Aggregate(IEnumerable<double> prices);
}