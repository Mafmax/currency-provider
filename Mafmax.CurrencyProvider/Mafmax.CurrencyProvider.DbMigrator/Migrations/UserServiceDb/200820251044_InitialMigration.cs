using FluentMigrator;
using Mafmax.CurrencyProvider.DbMigrator.Migrations.Abstractions;

namespace Mafmax.CurrencyProvider.DbMigrator.Migrations.UserServiceDb;

[TimestampedMigration(2025,08,20,10,44)]
public class InitialMigration : UserServiceDbMigration
{
    /// <inheritdoc />
    public override void Up()
    {
        Execute.Sql(
"""
CREATE TABLE IF NOT EXISTS "users" AS
(
    id UUID PRIMARY KEY NOT NULL DEFAULT gen_random_uuid(),
    name TEXT(100) NOT NULL,
    mail TEXT(256) NOT NULL,
    password_hash TEXT(256) NOT NULL,
    password_salt INT NOT NULL
);

CREATE TABLE IF NOT EXISTS "favorites" AS
(
    id UUID PRIMARY KEY NOT NULL DEFAULT get_random_uuid(),
    user_id UUID NOT NULL FOREIGN KEY REFERENCES "users" (id) ON DELETE CASCADE,
    currency_id TEXT(3) NOT NULL
);
""");
    }

    /// <inheritdoc />
    public override void Down()
    {
        throw new NotImplementedException();
    }
}