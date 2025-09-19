using FluentMigrator.Runner;
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

public static class Program
{
    public static void Main(string[] args)
    {
        if (args.Contains("--dryrun"))
        {
            return;
        }

        // Получаем переменную среды, отвечающую за окружение
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ??
                              throw new InvalidOperationException("ASPNETCORE_ENVIRONMENT in not set");

        // собираем конфигурацию на основании окружения
        // у нас будет два варианта - Development/Production
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"appsettings.{environmentName}.json")
            .Build();

        // Получаем строку подключения из конфига `appsettings.{Environment}.json`
        var connectionString = config["DbSettings:MigrationConnectionString"];
        var migrationRunner = new MigratorRunner(connectionString);
        
        // Мигрируемся
        migrationRunner.Migrate();
    }
}

public class MigratorRunner(string connectionString)
{
    public void Migrate()
    {
        var serviceProvider = CreateServices();

        using var scope = serviceProvider.CreateScope();
        UpdateDatabase(serviceProvider.GetRequiredService<IMigrationRunner>());
    }

    private IServiceProvider CreateServices()
    {
        Console.WriteLine(typeof(MigratorRunner).Assembly.FullName);
        
        // Зависимости
        // Хотим fluentMigrator с постгресом
        // и чтобы искал миграции в текущем проекте.
        // Также добавляем консольное логирование и
        // собственную реализацию интерфейса IVersionTableMetaData 
        // (которая хранит накаченные миграции) 
        return new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(rb => rb
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .ScanIn(typeof(MigratorRunner).Assembly).For.Migrations())
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .AddScoped<IVersionTableMetaData, VersionTable>()
            .BuildServiceProvider(false);
    }

    private void UpdateDatabase(IMigrationRunner runner)
    {
        // Мигрируем базу
        runner.MigrateUp();
        // создаем и открываем коннект к бд
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        // перегружаем композитные типы
        connection.ReloadTypes();
    }
}

public class VersionTable : IVersionTableMetaData
{
    public bool OwnsSchema => true;

    public string SchemaName => "public";

    public string TableName => "version_info";

    public string ColumnName => "version";

    public string DescriptionColumnName => "description";

    public string AppliedOnColumnName => "applied_on";

    public bool CreateWithPrimaryKey { get; } = false;

    public string UniqueIndexName => "uc_version";
}