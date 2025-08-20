using FluentMigrator;

namespace Mafmax.CurrencyProvider.DbMigrator.Maintenances;

/// <summary>
/// Represents lock releasing for migration process.
/// </summary>
/// <param name="lockId">Need be the same as passed into <see cref="Lock"/>.</param>
public class Unlock(int lockId) : Migration
{
    /// <inheritdoc />
    public override void Up() => Execute.Sql($"SELECT pg_advisory_unlock({lockId});");

    /// <inheritdoc />
    public override void Down() => throw new NotImplementedException();
}