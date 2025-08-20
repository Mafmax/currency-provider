using FluentMigrator.Runner;
using Mafmax.CurrencyProvider.DbMigrator.Maintenances;

namespace Mafmax.CurrencyProvider.DbMigrator.Extensions;

public static class MigrationRunnerExtensions
{
    /// <summary>
    /// Some unique value that shared between apps to prevent parallel migration process.
    /// </summary>
    private const int LOCK_ID = 1234567890;
    public static void MigrateWithLock(this IMigrationRunner runner)
    {
        runner.Processor.BeginTransaction();
        runner.Up(new Lock(LOCK_ID));

        try
        {
            runner.MigrateUp();
        }
        finally
        {
            runner.Up(new Unlock(LOCK_ID));
        }
    }
}