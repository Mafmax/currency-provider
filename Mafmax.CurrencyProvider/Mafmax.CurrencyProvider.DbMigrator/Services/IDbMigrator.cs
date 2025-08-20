namespace Mafmax.CurrencyProvider.DbMigrator.Services;

public interface IDbMigrator
{
    public Task MigrateAsync(MigrationOption migrationOption, CancellationToken ct);
}