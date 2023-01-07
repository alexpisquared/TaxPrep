# 2023-01-07  Project Creation
## Useful information 
### Useful information 
#### Useful information 

dotnet new classlib -n Db.FinDemo7
cd .\Db.FinDemo7\
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet ef dbcontext scaffold "Server=.\SqlExpress;Database=FinDemo;Trusted_Connection=True;TrustServerCertificate=Yes;" Microsoft.EntityFrameworkCore.SqlServer -o Models --force

-- Detached and removed the database to succedd restore from the latest RLS:
USE [master]
RESTORE DATABASE [FinDemoDbg] FROM  DISK = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\Backup\FinDemo.bak' WITH  FILE = 1,  MOVE N'FinDemo' TO N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\FinDemoDbg.mdf',  MOVE N'FinDemo_log' TO N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\FinDemoDbg_log.ldf',  NORECOVERY,  NOUNLOAD,  REPLACE,  STATS = 5
RESTORE DATABASE [FinDemoDbg] FROM  DISK = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\Backup\FinDemo.bak' WITH  FILE = 2,  NORECOVERY,  NOUNLOAD,  STATS = 5
GO
RESTORE DATABASE [FinDemoDbg] WITH  RECOVERY -- !!!!!!!!!!!!!!!! if stuck in RESTORING state.

