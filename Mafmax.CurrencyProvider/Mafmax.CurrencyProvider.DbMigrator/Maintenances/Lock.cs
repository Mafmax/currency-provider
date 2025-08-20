using FluentMigrator;

namespace Mafmax.CurrencyProvider.DbMigrator.Maintenances;

/// <summary>
/// Represents lock acquiring for migration process.
/// </summary>
public class Lock(int lockId) : Migration
{
    /// <inheritdoc />
    public override void Up() => Execute.Sql($"SELECT pg_advisory_lock({lockId});");

    /// <inheritdoc />
    public override void Down() => throw new NotImplementedException();
}