use UserDataDB
SELECT name FROM sys.internal_tables where name like '%filestream%' --filestream_tombstone_2073058421
GO
BACKUP LOG [UserDataDB] TO  DISK = N'D:\Program Files\Microsoft SQL Server\MSSQL16.MSSQLSERVER\MSSQL\Backup\UserDataDB.bak' 
WITH NOFORMAT, NOINIT,  NAME = N'UserDataDB-Full Database Backup', SKIP, NOREWIND, NOUNLOAD,  STATS = 10
GO
EXEC sp_filestream_force_garbage_collection UserDataDB
Go