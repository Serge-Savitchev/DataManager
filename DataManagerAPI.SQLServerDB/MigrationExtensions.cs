using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;

namespace DataManagerAPI.SQLServerDB;

/// <summary>
/// Extensions for setup FILESTREAM feature.
/// </summary>
public static class MigrationExtensions
{
    /// <summary>
    /// Prepare database and table for FILESTREAM support.
    /// </summary>
    /// <param name="migrationBuilder"><see cref="MigrationBuilder"/></param>
    public static void SetUpFileStreamFeature(MigrationBuilder migrationBuilder)
    {
        // take database name from connection string
        var dbName = ExtructDBNameFromConnectionString(GetConnectionString());

        // default name of the table
        const string tableName = "dbo.UserFiles";

        // create (if not exists) directorty for FILESTERAM data
        var dataPath = $"D:\\Program Files\\Microsoft SQL Server\\MSSQL16.MSSQLSERVER\\MSSQL\\DATA\\{dbName}\\{dbName}Stream";

        // create directory for FILESTREAM files
        Directory.CreateDirectory(dataPath);

        // setup FILESTREAM feature support
        migrationBuilder.Sql($"ALTER DATABASE {dbName} ADD FILEGROUP [{dbName}Stream] CONTAINS FILESTREAM", true);
        migrationBuilder.Sql($"ALTER DATABASE {dbName} ADD FILE ( NAME = N'{dbName}Files', FILENAME = N'{dataPath}\\{dbName}Files') TO FILEGROUP [{dbName}Stream]", true);

        // add colunms 'FileId', 'Content', 'Data' for FILESTREAM support
        migrationBuilder.Sql($"ALTER TABLE {tableName} ADD FileId UNIQUEIDENTIFIER ROWGUIDCOL UNIQUE NOT NULL", true);
        migrationBuilder.Sql($"ALTER TABLE {tableName} ADD Content VARBINARY(MAX) FILESTREAM", true);

        // this column is used for storing data of "small" files. It is alternative of FILESTREAM feature.
        migrationBuilder.Sql($"ALTER TABLE {tableName} ADD Data VARBINARY(MAX)", true);
    }

    public static string GetConnectionString()
    {
        // Get environment
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")!;

        // Build config
        var builder = new ConfigurationBuilder();
        builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
        builder.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false);

        IConfigurationRoot config = builder.Build();

        // Get connection string
        var connectionString = config.GetConnectionString("SQLServerDB")!;

        Console.WriteLine($"Environment: {environment}");
        Console.WriteLine(connectionString);

        return connectionString;
    }

    /// <summary>
    /// Extructs database name from connection string.
    /// </summary>
    /// <param name="connectionString"></param>
    /// <returns>Database name.</returns>
    public static string ExtructDBNameFromConnectionString(string connectionString)
    {
        var tmp = connectionString.Replace(" ", "");
        int beginIndex = tmp.IndexOf("Database=", 0, StringComparison.InvariantCultureIgnoreCase) + "Database=".Length;
        int endIndex = tmp.IndexOf(";", beginIndex, StringComparison.InvariantCultureIgnoreCase);
        if (endIndex < 0)
        {
            endIndex = tmp.Length;
        }

        return tmp.Substring(beginIndex, endIndex - beginIndex);
    }
}
