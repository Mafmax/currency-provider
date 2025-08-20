using FluentMigrator;

namespace Mafmax.CurrencyProvider.DbMigrator.Migrations.Abstractions;

[FluentMigrator.Tags("finance-service-db")]
public abstract class FinanceServiceDbMigration : Migration;


[FluentMigrator.Tags("user-service-db")]
public abstract class UserServiceDbMigration : Migration;