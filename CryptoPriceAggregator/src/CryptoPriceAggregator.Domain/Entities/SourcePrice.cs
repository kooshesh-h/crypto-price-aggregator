
namespace CryptoPriceAggregator.Domain.Entities;

public class SourcePrice
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid AggregatedPriceId { get; private set; }

    public string SourceName { get; private set; } = string.Empty;

    public double Price { get; private set; }

    private SourcePrice()
    {
    }

    public SourcePrice(string sourceName, double price)
    {
        if (string.IsNullOrWhiteSpace(sourceName))
            throw new ArgumentException("Source name is required.", nameof(sourceName));

        SourceName = sourceName;
        Price = price;
    }
}