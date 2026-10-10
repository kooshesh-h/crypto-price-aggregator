using System.Text.Json;
using CryptoPriceAggregator.Application.Abstractions.Pricing;
using CryptoPriceAggregator.Domain.Entities;

namespace CryptoPriceAggregator.Infrastructure.Pricing.Sources;

public class BitfinexPriceSource : IPriceSource
{
    private readonly HttpClient _httpClient;

    public BitfinexPriceSource(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public string Name => "Bitfinex";

    public async Task<SourcePrice> GetPriceAsync(
        string instrument,
        DateTime timestampUtc,
        CancellationToken cancellationToken = default)
    {
        var symbol = $"t{instrument.Replace("/", "").ToUpperInvariant()}";

        var startTimestamp = new DateTimeOffset(timestampUtc)
            .ToUnixTimeMilliseconds();

        var endTimestamp = new DateTimeOffset(timestampUtc.AddHours(1))
            .ToUnixTimeMilliseconds();

        var requestUrl =
            $"candles/trade:1h:{symbol}/hist" +
            $"?start={startTimestamp}&end={endTimestamp}&limit=1&sort=1";

        using var response = await _httpClient.GetAsync(
            requestUrl,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        using var document = JsonDocument.Parse(json);

        var candle = document.RootElement.EnumerateArray().FirstOrDefault();

        if (candle.ValueKind == JsonValueKind.Undefined ||
            candle.GetArrayLength() < 2)
        {
            throw new InvalidOperationException(
                "Bitfinex did not return a price candle.");
        }

        var closePrice = candle[2].GetDouble();

        return new SourcePrice(Name, closePrice);
    }
}