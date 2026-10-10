
namespace CryptoPriceAggregator.Domain.Entities;

public class AggregatedPrice
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Instrument { get; private set; } = string.Empty;

    public DateTime TimestampUtc { get; private set; }

    public double Price { get; private set; }

    public List<SourcePrice> SourcePrices { get; private set; } = [];

    private AggregatedPrice()
    {
    }

    public AggregatedPrice(
        string instrument,
        DateTime timestampUtc,
        double price,
        IEnumerable<SourcePrice> sourcePrices)
    {
        if (string.IsNullOrWhiteSpace(instrument))
            throw new ArgumentException("Instrument is required.", nameof(instrument));

        if (timestampUtc.Minute != 0 ||
            timestampUtc.Second != 0 ||
            timestampUtc.Millisecond != 0)
        {
            throw new ArgumentException(
                "Timestamp must have hour accuracy.",
                nameof(timestampUtc));
        }

        Instrument = instrument.Trim().ToUpperInvariant();
        TimestampUtc = DateTime.SpecifyKind(timestampUtc, DateTimeKind.Utc);
        Price = price;
        SourcePrices = sourcePrices.ToList();
    }
}
