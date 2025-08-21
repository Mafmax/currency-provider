using FluentMigrator.Runner;
using FluentMigrator.Runner.Exceptions;
using FluentMigrator.Runner.Initialization;
using Mafmax.CurrencyProvider.DbMigrator.Extensions;
using Npgsql;
using Polly;

namespace Mafmax.CurrencyProvider.DbMigrator.Services;

public class DbMigrator(ILogger<DbMigrator> logger) : IDbMigrator
{
    private readonly Policy _retryPolicy = Policy
        .Handle<Exception>(x => x.GetType() != typeof(MissingMigrationsException))
        .WaitAndRetry(retryCount: 3,
            sleepDurationProvider: attempt => TimeSpan.FromMinutes(3),
            onRetry: (ex, timeSpan, retryCount, ctx) =>
            {
                logger.LogWarning(ex,
                    """
                    Migration attempt {RetryCount} failed. New try after {RetryAfter} seconds.
                    Connection string was {ConnectionString}. Tags were {MigrationTags}.
                    """,
                    retryCount, timeSpan.TotalSeconds, ClearConnectionString(ctx["ConnectionString"].ToString()), ctx["Tags"]);
            });

    /// <inheritdoc />
    public async Task MigrateAsync(MigrationOption migrationOption, CancellationToken ct)
    {
        using var logScope = logger.BeginScope(
            "Starting migration process for DB with connection ({ConnectionString}) with tags {MigrationRunnerTags}.",
            ClearConnectionString(migrationOption.ConnectionString), migrationOption.Tags);

        try
        {
            await Task.Run(() => MigrateInner(migrationOption.ConnectionString, migrationOption.Tags), ct);
            logger.LogInformation("Migration completed.");
        }
        catch (MissingMigrationsException ex)
        {
            logger.LogWarning(ex, "No migrations found.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Migration process failed.");

            throw;
        }
    }
    private static string ClearConnectionString(string? connectionString) =>
        new NpgsqlConnectionStringBuilder(connectionString)
        {
            Password = "***",
        }.ConnectionString;

    private void MigrateInner(string connectionString, string[] runnerTags)
    {
        var migrationRunner = new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddPostgres11_0()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(DbMigrator).Assembly).For.Migrations()
                .WithGlobalCommandTimeout(TimeSpan.FromMinutes(5)))
            .Configure<RunnerOptions>(x =>
            {
                x.Tags = runnerTags;
                x.TransactionPerSession = true;
            })
            .BuildServiceProvider(false)
            .GetRequiredService<IMigrationRunner>();

        var retryContext = new Context("Migration", new Dictionary<string, object>
        {
            ["ConnectionString"] = connectionString,
            ["Tags"] = runnerTags
        });

        _retryPolicy.Execute(_ => migrationRunner.MigrateWithLock(), retryContext);
    }
}