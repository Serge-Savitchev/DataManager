using Microsoft.EntityFrameworkCore.Migrations;

namespace DataManagerAPI.PostgresDB;

/// <summary>
/// Extensions for setup Postgres database for working with big files.
/// </summary>
public static class PostgresMigrationExtensions
{
    /// <summary>
    /// Prepare database and table for FILESTREAM support.
    /// </summary>
    /// <param name="migrationBuilder"><see cref="MigrationBuilder"/></param>
    public static void SetUpBlobFeature(MigrationBuilder migrationBuilder)
    {
        // default name of the table
        const string tableName = "UserFiles";

        // add colunms 'Oid' for big files support.
        migrationBuilder.Sql($"""ALTER TABLE IF EXISTS public."{tableName}" ADD COLUMN "Oid" bigint NOT NULL;""", true);

        // this column is used for storing data of "small" files.
        migrationBuilder.Sql($"""ALTER TABLE IF EXISTS public."{tableName}" ADD COLUMN "Data" bytea;""", true);
    }
}
