# Crypto Price Aggregator

Crypto Price Aggregator is a .NET and Angular application that aggregates BTC/USD hourly close prices from Bitstamp and Bitfinex. For a requested instrument and UTC hour, the API calculates the average and persists the result with its source prices in SQLite. A later request for the same instrument and UTC hour returns the persisted result instead of calling the providers again.

## Features

- Request an aggregated BTC/USD price for a specific UTC hour.
- Retrieve previously persisted prices within a UTC time range.
- Calculate the arithmetic average of the source close prices and show the Bitstamp and Bitfinex prices in the response.
- Persist aggregated and source prices in SQLite for subsequent requests.
- Explore the REST API through Swagger in development.
- Use an Angular frontend that shows the result, source prices, cache or external-provider status, price history, and loading and error states.

## Architecture and project structure

The solution follows clean architecture boundaries:

/ Project / Responsibility /
/ --- / --- /
/ `src/CryptoPriceAggregator.Domain` / Aggregated and source price entities. /
/ `src/CryptoPriceAggregator.Application` / Price queries, handlers, abstractions, and average aggregation logic. /
/ `src/CryptoPriceAggregator.Infrastructure` / Bitstamp and Bitfinex clients, Entity Framework Core context, and SQLite repository. /
/ `src/CryptoPriceAggregator.Api` / ASP.NET Core REST endpoints, dependency injection, and Swagger setup. /
/ `src/CryptoPriceAggregator.Web` / Standalone Angular frontend and development proxy. /
/ `tests/CryptoPriceAggregator.Application.Tests` / Backend unit tests for the application layer. /

The application uses CQRS-style query and handler classes without MediatR.

## Technology stack

- .NET 10 and ASP.NET Core REST API
- Entity Framework Core with SQLite
- Angular standalone frontend
- xUnit for backend unit tests
- Swashbuckle / Swagger for API documentation

## API endpoints

/ Endpoint / Query parameters / Purpose /
/ --- / --- / --- /
/ `GET /api/prices/aggregated` / `instrument`, `timestampUtc` / Get or calculate the aggregated price for one UTC hour. /
/ `GET /api/prices/history` / `instrument`, `fromUtc`, `toUtc` / Get persisted prices for the instrument in the inclusive UTC range. /

For the aggregated-price request, the timestamp must be UTC and have hour accuracy: minutes, seconds, and milliseconds must be zero. Use a `Z` suffix, for example `2025-10-01T10:00:00Z`. The history bounds must also be UTC, and `fromUtc` must be earlier than or equal to `toUtc`. Use hourly UTC bounds to select complete hours.

Examples for `BTC/USD`:

```text
GET https://localhost:7292/api/prices/aggregated?instrument=BTC%2FUSD&timestampUtc=2025-10-01T10%3A00%3A00Z
GET https://localhost:7292/api/prices/history?instrument=BTC%2FUSD&fromUtc=2025-10-01T10%3A00%3A00Z&toUtc=2025-10-01T12%3A00%3A00Z
```

The aggregated response includes `instrument`, `timestampUtc`, `price`, `isFromCache`, and `sourcePrices`. Each source price includes its provider name and close price. The history response contains matching persisted items and their source prices; it does not fetch missing hours from providers.

## Cache behavior

On the first request for an instrument and UTC hour, the API fetches the hourly close price from each configured external provider, calculates their arithmetic average, persists the aggregated and source prices, and returns `isFromCache: false`. A repeated request for the same instrument and UTC hour reads the saved data from SQLite and returns `isFromCache: true`.

## Run locally

Prerequisites: .NET 10 SDK, Node.js, and npm. Run the following commands in two terminals. The backend command starts in the repository root:

**Terminal 1 — backend**

```powershell
dotnet run --project src/CryptoPriceAggregator.Api --launch-profile https
```

Swagger is available at <https://localhost:7292/swagger> while the API runs in the development environment.

**Terminal 2 — frontend**

```powershell
cd src/CryptoPriceAggregator.Web
npm install
npm start
```

Open <http://localhost:4200>. The Angular development proxy forwards `/api` requests to the local ASP.NET Core API at `https://localhost:7292`, so local development does not require CORS setup. The first uncached request needs network access to Bitstamp and Bitfinex.

## Database

The API uses SQLite. On startup it creates the database automatically if it does not already exist, including in development. The configured connection string is `Data Source=crypto-prices.db`; the file is created relative to the API process's working directory. The local `.db` file is runtime data and should not be committed to the repository.

## Testing

From the repository root, run:

```powershell
dotnet test
```

The backend unit test covers the arithmetic average-price aggregation calculation using two source prices.

## Assumptions

- This prototype currently supports BTC/USD.
- The aggregation formula is the arithmetic average of the available source close prices.
- Provider data uses one-hour OHLC candles; only the close price is used.
- The source interface and aggregation abstraction provide extension points for additional price providers or a different aggregation formula.

## Submission note

The submission repository should be private, with access granted to GitHub user `tradingpoint`.
