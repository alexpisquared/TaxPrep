USE [FinDemo]
GO
/****** Object:  View [dbo].[Vw_TaxLiqReport*]    Script Date: 2023-01-29 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[Vw_TaxLiqReport_Alx]
AS
SELECT     TOP (100) PERCENT dbo.ExpenseGroup.Name AS [Group], dbo.TxCategory.TL_Number, dbo.TxCategory.Name AS Category, CASE WHEN TxCategory.DeductiblePercentage = 1 THEN '' ELSE CAST(ROUND(SUM(TxCoreV2.TxAmount), 2) AS varchar(9)) 
                  + ' * ' + CAST(100 * TxCategory.DeductiblePercentage AS varchar(15)) + '%' END AS PartCalcShow, ROUND(SUM(dbo.TxCategory.DeductiblePercentage * dbo.TxCoreV2.TxAmount), 2) AS TtlExp, MIN(dbo.TxCoreV2.TxDate) AS FirstTx, MAX(dbo.TxCoreV2.TxDate) AS LastTx, COUNT(*) AS Qnt, 
                  dbo.TxMoneySrc.Owner
FROM        dbo.ExpenseGroup INNER JOIN
                  dbo.TxCategory ON dbo.ExpenseGroup.Id = dbo.TxCategory.ExpGroupId INNER JOIN
                  dbo.TxCoreV2 ON dbo.TxCategory.IdTxt = dbo.TxCoreV2.TxCategoryIdTxt INNER JOIN
                  dbo.TxMoneySrc ON dbo.TxCoreV2.TxMoneySrcId = dbo.TxMoneySrc.Id
WHERE     (dbo.TxCoreV2.TxAmount <> 0) AND (dbo.TxCoreV2.TxDate BETWEEN CONVERT(DATETIME, '2022-01-01 00:00:00', 102) AND CONVERT(DATETIME, '2022-12-31 23:59:59', 102))
GROUP BY dbo.ExpenseGroup.Note, dbo.ExpenseGroup.Name, dbo.TxCategory.TL_Number, dbo.TxCategory.Name, dbo.TxCategory.DeductiblePercentage, dbo.ExpenseGroup.Id, dbo.TxMoneySrc.Owner
HAVING     (dbo.TxMoneySrc.Owner = 'Alx') AND (dbo.TxCategory.TL_Number IS NOT NULL) AND (dbo.TxCategory.TL_Number < 200)
ORDER BY dbo.ExpenseGroup.Note, ISNULL(dbo.TxCategory.TL_Number, 999)
GO

ALTER VIEW [dbo].[Vw_TaxLiqReport_Mei]
AS
SELECT     TOP (100) PERCENT dbo.ExpenseGroup.Name AS [Group], dbo.TxCategory.TL_Number, dbo.TxCategory.Name AS Category, CASE WHEN TxCategory.DeductiblePercentage = 1 THEN '' ELSE CAST(ROUND(SUM(TxCoreV2.TxAmount), 2) AS varchar(9)) 
                  + ' * ' + CAST(100 * TxCategory.DeductiblePercentage AS varchar(15)) + '%' END AS PartCalcShow, ROUND(SUM(dbo.TxCategory.DeductiblePercentage * dbo.TxCoreV2.TxAmount), 2) AS TtlExp, MIN(dbo.TxCoreV2.TxDate) AS FirstTx, MAX(dbo.TxCoreV2.TxDate) AS LastTx, COUNT(*) AS Qnt, 
                  dbo.TxMoneySrc.Owner
FROM        dbo.ExpenseGroup INNER JOIN
                  dbo.TxCategory ON dbo.ExpenseGroup.Id = dbo.TxCategory.ExpGroupId INNER JOIN
                  dbo.TxCoreV2 ON dbo.TxCategory.IdTxt = dbo.TxCoreV2.TxCategoryIdTxt INNER JOIN
                  dbo.TxMoneySrc ON dbo.TxCoreV2.TxMoneySrcId = dbo.TxMoneySrc.Id
WHERE     (dbo.TxCoreV2.TxAmount <> 0) AND (dbo.TxCoreV2.TxDate BETWEEN CONVERT(DATETIME, '2022-01-01 00:00:00', 102) AND CONVERT(DATETIME, '2022-12-31 23:59:59', 102))
GROUP BY dbo.ExpenseGroup.Note, dbo.ExpenseGroup.Name, dbo.TxCategory.TL_Number, dbo.TxCategory.Name, dbo.TxCategory.DeductiblePercentage, dbo.ExpenseGroup.Id, dbo.TxMoneySrc.Owner
HAVING     (dbo.TxMoneySrc.Owner = 'Mei') AND (dbo.TxCategory.TL_Number IS NOT NULL) AND (dbo.TxCategory.TL_Number < 200)
ORDER BY dbo.ExpenseGroup.Note, ISNULL(dbo.TxCategory.TL_Number, 999)
GO

ALTER VIEW [dbo].[Vw_TaxLiqReport]
AS
SELECT     TOP (100) PERCENT dbo.ExpenseGroup.Name AS [Group], dbo.TxCategory.TL_Number, dbo.TxCategory.Name AS Category, CASE WHEN TxCategory.DeductiblePercentage = 1 THEN '' ELSE CAST(ROUND(SUM(TxCoreV2.TxAmount), 2) AS varchar(9)) 
                  + ' * ' + CAST(100 * TxCategory.DeductiblePercentage AS varchar(15)) + '%' END AS PartCalcShow, ROUND(SUM(dbo.TxCategory.DeductiblePercentage * dbo.TxCoreV2.TxAmount), 2) AS TtlExp, MIN(dbo.TxCoreV2.TxDate) AS FirstTx, MAX(dbo.TxCoreV2.TxDate) AS LastTx, COUNT(*) 
                  AS Qnt
FROM        dbo.ExpenseGroup INNER JOIN
                  dbo.TxCategory ON dbo.ExpenseGroup.Id = dbo.TxCategory.ExpGroupId INNER JOIN
                  dbo.TxCoreV2 ON dbo.TxCategory.IdTxt = dbo.TxCoreV2.TxCategoryIdTxt
WHERE     (dbo.TxCoreV2.TxAmount <> 0) AND (dbo.TxCoreV2.TxDate BETWEEN CONVERT(DATETIME, '2022-01-01 00:00:00', 102) AND CONVERT(DATETIME, '2022-12-31 23:59:59', 102))
GROUP BY dbo.ExpenseGroup.Note, dbo.ExpenseGroup.Name, dbo.TxCategory.TL_Number, dbo.TxCategory.Name, dbo.TxCategory.DeductiblePercentage, dbo.ExpenseGroup.Id
ORDER BY dbo.ExpenseGroup.Note, ISNULL(dbo.TxCategory.TL_Number, 999)
GO
/*
select * from [Vw_TaxLiqReport_Alx]
select * from [Vw_TaxLiqReport_Mei]
select * from [Vw_TaxLiqReport]
*/