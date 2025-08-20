using Mafmax.CurrencyProvider.DbMigrator;
using Mafmax.CurrencyProvider.DbMigrator.Services;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureServices();

builder.Services.AddSerilog(cfg =>
{
    cfg
        .Enrich.FromLogContext()
        .WriteTo.Console(LogEventLevel.Information)
        .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day);
});

var settings = builder.Configuration.GetSection(nameof(ApplicationSettings)).Get<ApplicationSettings>() ??
               throw new InvalidOperationException("Application settings not found in configuration.");

var app = builder.Build();

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

await MigrateAsync(app.Services, settings.MigrationOptions, CancellationToken.None);

app.Run();

async Task MigrateAsync(IServiceProvider services, MigrationOption[] migrationOptions, CancellationToken ct)
{
    if(migrationOptions.Length == 0 ) return;

    await using var scope = services.CreateAsyncScope();

    var migrator = scope.ServiceProvider.GetRequiredService<IDbMigrator>();

    var migrateTasks = migrationOptions.Select(mo => migrator.MigrateAsync(mo, ct)).ToArray();
    
    await Task.WhenAll(migrateTasks);
}
