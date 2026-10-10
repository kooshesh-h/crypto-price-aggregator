using CryptoPriceAggregator.Application.Abstractions.Persistence;
using CryptoPriceAggregator.Infrastructure.Data;
using CryptoPriceAggregator.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CryptoPriceAggregator.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));

        services.AddScoped<IPriceRepository, PriceRepository>();

        return services;
    }
}