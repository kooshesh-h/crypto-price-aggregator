using System.Globalization;
using System.Text.Json;
using CryptoPriceAggregator.Application.Abstractions.Pricing;
using CryptoPriceAggregator.Domain.Entities;

namespace CryptoPriceAggregator.Infrastructure.Pricing.Sources;

public class BitstampPriceSource : IPriceSource
{
    private readonly HttpClient _httpClient;

    public BitstampPriceSource(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public string Name => "Bitstamp";

    public async Task<SourcePrice> GetPriceAsync(
        string instrument,
        DateTime timestampUtc,
        CancellationToken cancellationToken = default)
    {
        var marketSymbol = instrument.Replace("/", "").ToLowerInvariant();
        var unixTimestamp = new DateTimeOffset(timestampUtc)
            .ToUnixTimeSeconds();

        var requestUrl =
            $"api/v2/ohlc/{marketSymbol}/?step=3600&limit=1&start={unixTimestamp}";

        using var response = await _httpClient.GetAsync(
            requestUrl,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken);

        using var document = JsonDocument.Parse(json);

        var candles = document.RootElement
            .GetProperty("data")
            .GetProperty("ohlc");

        var candle = candles.EnumerateArray().FirstOrDefault();

        if (candle.ValueKind == JsonValueKind.Undefined)
            throw new InvalidOperationException(
                "Bitstamp did not return a price candle.");

        var closePriceText = candle.GetProperty("close").GetString();

        if (!double.TryParse(
                closePriceText,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out var closePrice))
        {
            throw new InvalidOperationException(
                "Bitstamp returned an invalid close price.");
        }

        return new SourcePrice(Name, closePrice);
    }
}