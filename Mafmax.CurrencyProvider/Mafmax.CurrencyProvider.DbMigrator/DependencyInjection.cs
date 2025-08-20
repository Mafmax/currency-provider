using Mafmax.CurrencyProvider.DbMigrator.Services;

namespace Mafmax.CurrencyProvider.DbMigrator;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services) =>
        services.AddSingleton<IDbMigrator, Services.DbMigrator>();
}