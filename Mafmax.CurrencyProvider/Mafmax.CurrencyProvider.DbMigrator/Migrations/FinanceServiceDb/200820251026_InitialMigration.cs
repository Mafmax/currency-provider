using FluentMigrator;
using Mafmax.CurrencyProvider.DbMigrator.Migrations.Abstractions;

namespace Mafmax.CurrencyProvider.DbMigrator.Migrations.FinanceServiceDb;

[TimestampedMigration(2025, 08, 12, 10, 26)]
public class InitialMigration : FinanceServiceDbMigration
{
    /// <inheritdoc />
    public override void Up()
    {
        Execute.Sql(
"""
CREATE TABLE IF NOT EXISTS "currencies" AS
(
    id TEXT(3) PRIMARY KEY NOT NULL,
    name TEXT(40) NOT NULL,
    rate: NUMERIC(20, 10) NOT NULL,
    date: TIMESTAMP NOT NULL
);

CREATE INDEX IF NOT EXISTS "currencies" ON "currencies"
USING BTREE (id, date);

CREATE TABLE IF NOT EXISTS "syncs" AS
(
    id UUID PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
    date TIMESTAMP NOT NULL
);
""");
    }

    /// <inheritdoc />
    public override void Down()
    {
        throw new NotImplementedException();
    }
}