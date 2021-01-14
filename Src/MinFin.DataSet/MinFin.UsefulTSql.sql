USE [FinDemo]
go

--Backup of the Vw_TaxLiqReport:
SELECT     TOP (100) PERCENT dbo.ExpenseGroup.Name AS [Group], dbo.TxCategory.TL_Number, dbo.TxCategory.Name AS Category, 
                      CASE WHEN TxCategory.DeductiblePercentage = 1 THEN '' ELSE CAST(ROUND(SUM(TxCore.TxAmount), 2) AS varchar(9)) 
                      + ' * ' + CAST(100 * TxCategory.DeductiblePercentage AS varchar(5)) + '% ' END AS PartCalcShow, 
                      ROUND(SUM(dbo.TxCategory.DeductiblePercentage * dbo.TxCore.TxAmount), 2) AS TtlExp, COUNT(*) AS Qnt, MAX(dbo.TxCore.TxDate) AS LastTx
FROM         dbo.ExpenseGroup INNER JOIN
                      dbo.TxCategory ON dbo.ExpenseGroup.Id = dbo.TxCategory.ExpGroupId INNER JOIN
                      dbo.TxCore ON dbo.TxCategory.IdTxt = dbo.TxCore.TxCategoryIdTxt
WHERE     (dbo.TxCore.TxAmount <> 0)
GROUP BY dbo.ExpenseGroup.Note, dbo.ExpenseGroup.Name, dbo.TxCategory.TL_Number, dbo.TxCategory.Name, dbo.TxCategory.DeductiblePercentage, 
                      dbo.ExpenseGroup.Id
ORDER BY dbo.ExpenseGroup.Note, ISNULL(dbo.TxCategory.TL_Number, 999)


/* One way only:
--From Hm:
BACKUP  DATABASE [FinDemo] TO    DISK = N'H:\1\bak\Sql.Bak\FinDemo.Hm.Latest.bak'  WITH NOFORMAT, NOINIT, NAME = N'FinDemo-Full Database Backup', SKIP, NOREWIND, NOUNLOAD,  STATS = 10

--To Wk:
USE [Master]
RESTORE DATABASE [FinDemo] FROM  DISK = N'O:\1\bak\Sql.Bak\FinDemo.Hm.Latest.bak'  WITH FILE = 1, MOVE N'FinDemo' TO N'C:\Program Files\Microsoft SQL Server\MSSQL10_50.SQL08\MSSQL\DATA\FinDemo.mdf',  MOVE N'FinDemo_log' TO N'C:\Program Files\Microsoft SQL Server\MSSQL10_50.SQL08\MSSQL\DATA\FinDemo.ldf',  NOUNLOAD,  REPLACE,  STATS = 10
*/
select top(22) * from TxCore order by TxCore.CreatedAt desc

--Alter VIEW [dbo].[Vw_TaxLiqReport]
--AS
SELECT     TOP (100) PERCENT dbo.ExpenseGroup.Name AS [Group], dbo.TxCategory.TL_Number, dbo.TxCategory.Name AS Category, 
                      CASE WHEN TxCategory.DeductiblePercentage = 1 THEN '' 
						  ELSE 
						  CAST(ROUND(SUM(TxCore.TxAmount), 2) AS varchar(9)) + ' * ' + 
						  CAST(100 * TxCategory.DeductiblePercentage AS varchar(5)) + '% '
                      END AS PartCalcShow,                       
                      ROUND(SUM(dbo.TxCategory.DeductiblePercentage * dbo.TxCore.TxAmount),
                       2) AS TtlExp, COUNT(*) AS Qnt, MAX(dbo.TxCore.TxDate) AS LastTx
FROM         dbo.ExpenseGroup INNER JOIN
                      dbo.TxCategory ON dbo.ExpenseGroup.Id = dbo.TxCategory.ExpGroupId INNER JOIN
                      dbo.TxCore ON dbo.TxCategory.IdTxt = dbo.TxCore.TxCategoryIdTxt
WHERE     (dbo.TxCore.TxAmount <> 0)
GROUP BY dbo.ExpenseGroup.Note, dbo.ExpenseGroup.Name, dbo.TxCategory.TL_Number, dbo.TxCategory.Name, dbo.TxCategory.DeductiblePercentage, 
                      dbo.ExpenseGroup.Id
ORDER BY dbo.ExpenseGroup.Note, ISNULL(dbo.TxCategory.TL_Number, 999)
GO


--grand total
--SELECT      SUM(TxAmount) AS [Grand Total] FROM         FinDemo.dbo.TxCore
--SELECT      SUM(TxAmount) AS [Grand Total] FROM         [FinDemo.Org].dbo.TxCore


-- 2010 Annual sub-totals
SELECT     TxCategory.Id, TxCategory.CreatedAt, TxCategory.Name, SUM(TxCore.TxAmount) AS Expr1
FROM         TxCategory INNER JOIN
                      TxCore ON TxCategory.Id = TxCore.TxCategoryId
WHERE     (TxCore.TxDate >= CONVERT(DATETIME, '2010-01-01 00:00:00', 102)) AND (TxCore.TxDate <= CONVERT(DATETIME, '2010-12-31 00:00:00', 102))
GROUP BY TxCategory.Id, TxCategory.CreatedAt, TxCategory.Name
ORDER BY TxCategory.Name
 
 
--TaxLiqud-rs entrying:
SELECT     ExpGroupId, IdTxt, Name, TL_Number, DeductiblePercentage, Notes,
                          (SELECT     SUM(TxAmount) AS Expr1
                            FROM          TxCore
                            WHERE      (TxCategory.Id = TxCategoryId)) AS Ttl,
                          (SELECT     COUNT(TxAmount) AS Expr1
                            FROM          TxCore AS TxCore_1
                            WHERE      (TxCategory.Id = TxCategoryId)) AS Qnt,
                          (SELECT     SUM(TxAmount) AS Expr1
                            FROM          TxCore
                            WHERE      (TxCategory.IdTxt = TxCategoryIdTxt)) AS Ttl,
                          (SELECT     COUNT(TxAmount) AS Expr1
                            FROM          TxCore AS TxCore_1
                            WHERE      (TxCategory.IdTxt = TxCategoryIdTxt)) AS Qnt
FROM         TxCategory
ORDER BY ExpGroupId, TL_Number, Name


SELECT     TxCategoryIdTxt, TxSupplierId, TxEnvelopeId, TxAmount, TxAmountOrg, TxDate, ProductService, CreatedAt, ModifiedAt, Notes, History,
 (SELECT  Name       FROM   TxSupplier       WHERE  (TxCore.TxSupplierId = Id)) AS Supplier,
 (SELECT  Name       FROM   TxMoneySrc       WHERE  (TxCore.TxMoneySrcId = Id)) AS MoneySrc
FROM    TxCore
WHERE  (TxDate > CONVERT(DATETIME, '2010-01-01 00:00:00', 102)) AND (TxDate < CONVERT(DATETIME, '2011-01-01 00:00:00', 102)) 
--AND (TxAmount <> TxAmountOrg)
ORDER BY TxAmount DESC

--older:
SELECT     TxCategoryIdTxt, TxSupplierId, TxEnvelopeId, TxAmount, TxDate, ProductService, CreatedAt, ModifiedAt, Notes, History,
                          (SELECT     Name
                            FROM          TxSupplier
                            WHERE      (TxCore.TxSupplierId = Id)) AS Supplier
FROM         TxCore
WHERE     (TxCategoryIdTxt = 'FrnElec') AND (TxDate > CONVERT(DATETIME, '2010-01-01 00:00:00', 102))
ORDER BY TxAmount DESC



/*
--CRC
SELECT GETDATE() AS ToDayFor_All, SUM(TxAmount) AS AmtTotal, SUM(Id) AS SumId, SUM(TxMoneySrcId) AS SumMnySrcId, SUM(TxCategoryId) AS SumCtgId, SUM(TxSupplierId) AS SumSupId FROM TxCore 
SELECT GETDATE() AS ToDayFor2010, SUM(TxAmount) AS AmtTotal, SUM(Id) AS SumId, SUM(TxMoneySrcId) AS SumMnySrcId, SUM(TxCategoryId) AS SumCtgId, SUM(TxSupplierId) AS SumSupId FROM TxCore 
WHERE  (TxDate > CONVERT(DATETIME, '2010-01-01 00:00:00', 102)) AND (TxDate < CONVERT(DATETIME, '2011-01-01 00:00:00', 102)) 
	ToDay                   AmtTotal              SumId       SumMnySrcId SumCtgId    SumSupId
	----------------------- --------------------- ----------- ----------- ----------- -----------
Hm	2011-01-26 22:31:03.270 32493.70              41913       665         2959        9384
Wk	2011-01-27 09:09:56.963 32493.70              41913       665         2959        9384
Wk	2011-01-27 17:11:11.590 32493.70              42228       666         2958        9409
Hm	2011-01-27 18:34:13.897 32493.70              42228       666         2958        9409
Hm	2011-01-28 18:57:29.117 38672.62              53568       813         2895        12980
Hm	2011-01-30 11:19:34.530 38672.62              53568       813         2895        12980
Hm	2011-01-30 12:17:04.100 35838.47              54492       829         2857        13241 - removed Prop.tax duplicate
Hm	2011-01-30 22:54:07.540 47831.42              55297       877         2791        13691
Hm	2011-02-02 07:52:34.307 107744.38             59683       913         2779        14244 - Submitted to TaxLiq
Hm	2011-02-07 18:43:42.887 109627.53             54834       848         2699        13053 - 2010 only - Corrected and re-Submitted to TaxLiq
Hm	2011-02-07 18:43:42.887 116442.05             63067       927         2770        14996

*/

--UPDATE TxCore SET TxCategoryIdTxt = (SELECT IdTxt FROM TxCategory WHERE (TxCore.TxCategoryId = Id))
