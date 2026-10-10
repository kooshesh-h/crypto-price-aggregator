using CryptoPriceAggregator.Application.Abstractions.Pricing;
using CryptoPriceAggregator.Application.Features.Prices.Queries.GetAggregatedPrice;
using CryptoPriceAggregator.Application.Features.Prices.Queries.GetPriceHistory;
using CryptoPriceAggregator.Application.Services;
using CryptoPriceAggregator.Infrastructure;
using CryptoPriceAggregator.Infrastructure.Data;
using CryptoPriceAggregator.Infrastructure.Pricing.Sources;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' was not found.");

builder.Services.AddInfrastructure(connectionString);
builder.Services.AddSingleton<IPriceAggregator, AveragePriceAggregator>();
builder.Services.AddScoped<GetAggregatedPriceQueryHandler>();
builder.Services.AddScoped<GetPriceHistoryQueryHandler>();

builder.Services.AddHttpClient<BitstampPriceSource>(client =>
{
    client.BaseAddress = new Uri("https://www.bitstamp.net/");
    client.Timeout = TimeSpan.FromSeconds(15);
});

builder.Services.AddScoped<IPriceSource>(serviceProvider =>
    serviceProvider.GetRequiredService<BitstampPriceSource>());

builder.Services.AddHttpClient<BitfinexPriceSource>(client =>
{
    client.BaseAddress = new Uri("https://api-pub.bitfinex.com/v2/");
    client.Timeout = TimeSpan.FromSeconds(15);
});

builder.Services.AddScoped<IPriceSource>(serviceProvider =>
    serviceProvider.GetRequiredService<BitfinexPriceSource>());


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();