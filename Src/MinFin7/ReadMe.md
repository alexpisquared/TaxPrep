# 2023-01-07  Project Creation

dotnet new classlib -n Db.FinDemo7
cd .\Db.FinDemo7\
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet ef dbcontext scaffold "Server=.\SqlExpress;Database=FinDemo;Trusted_Connection=True;TrustServerCertificate=Yes;" Microsoft.EntityFrameworkCore.SqlServer -o Models --force

-- Detached and removed the database to succeed restore from the latest RLS:
USE [master]
RESTORE DATABASE [FinDemoDbg] FROM  DISK = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\Backup\FinDemo.bak' WITH  FILE = 1,  MOVE N'FinDemo' TO N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\FinDemoDbg.mdf',  MOVE N'FinDemo_log' TO N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\FinDemoDbg_log.ldf',  NORECOVERY,  NOUNLOAD,  REPLACE,  STATS = 5
RESTORE DATABASE [FinDemoDbg] FROM  DISK = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\Backup\FinDemo.bak' WITH  FILE = 2,  NORECOVERY,  NOUNLOAD,  STATS = 5
GO
RESTORE DATABASE [FinDemoDbg] WITH  RECOVERY -- !!!!!!!!!!!!!!!! if stuck in RESTORING state.

## Not used any more .. just for a fallback JIC.

# 2024-12-28  
Tried to solve  await _dbx.TxMoneySrcs.LoadAsync();  problem ... too difficult.

Regenerated the project and entities with the above script (nullability in the project did not help):

A copy from  C:\g\TaxPrep\Src\Db.FinDemo7.2024.12\Models\TxMoneySrc.cs  solved the above problem ... but there are some others:
.. so, keep replacing the entities with this^ if needed to solve the problem.

ACTUALLY, all the non .Net 4 projects have the issue.  The .Net 4 projects are OK.

