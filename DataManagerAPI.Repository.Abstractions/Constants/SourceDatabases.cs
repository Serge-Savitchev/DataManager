namespace DataManagerAPI.Repository.Abstractions.Constants;

/// <summary>
/// Description of configuration keys and values.
/// </summary>
public static class SourceDatabases
{
    /// <summary>
    /// Database type key. 
    /// </summary>
    public static string DatabaseType = "DatabaseType";

    /// <summary>
    /// Value of DatabaseType for SQL DB.
    /// </summary>
    public static string DatabaseTypeValueSQL = "SQL";

    /// <summary>
    /// Value of DatabaseType for Postgres DB.
    /// </summary>
    public static string DatabaseTypeValuePostgres = "Postgres";

    /// <summary>
    /// Key of flag of using GRPC service. Possible values: "true" or "false".
    /// </summary>
    public static string UseGPRC = "UseGPRC";

    /// <summary>
    /// Key for connection string for SQL database.
    /// </summary>
    public static string SQLConnectionString = "SQLConnectionString";

    /// <summary>
    /// Key for connection string for Postgres database.
    /// </summary>
    public static string PostgresConnectionString = "PostgresConnectionString";

    /// <summary>
    /// Key for connection string for GRPC service.
    /// </summary>
    public static string gRPCConnectionString = "gRPCConnectionString";
}
