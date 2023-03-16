### DataManagerAPI.PostgresDB

Class library.

It provides:
- migration of PostgreSQL database to server
- setting up possibity of storing big files in database
- processing requests to database

Connection strings for PostgreSQL batabase are defined in appsettings.*.json files of main application.

***Note**. It depends on DataManagerAPI.SQLServerDB project because requests to database via Entity Framework are the same and its are implementated in DataManagerAPI.SQLServerDB project.*
