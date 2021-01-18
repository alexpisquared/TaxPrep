USE [FinDemo] -- Script Date: 2021-01-17
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER VIEW [dbo].[Vw_Exp_Hist_vs_Last]
AS
SELECT    TOP (100) PERCENT ISNULL(dbo.TxCategory.TL_Number, 999) AS TaxLiq##, dbo.ExpenseGroup.Id AS ExpenseGroupId, dbo.ExpenseGroup.Name, dbo.TxCategory.IdTxt, dbo.TxCategory.Name AS Category,
			  (SELECT SUM(TxAmount) FROM    dbo.Vw_TxCore AS TxCore_2 WHERE    (TxCategoryIdTxt = tc.TxCategoryIdTxt) AND (YEAR(TxDate) = 2010) GROUP BY TxCategoryIdTxt) AS Exp2010,
			  (SELECT SUM(TxAmount) FROM    dbo.Vw_TxCore AS TxCore_1 WHERE    (TxCategoryIdTxt = tc.TxCategoryIdTxt) AND (YEAR(TxDate) = 2011) GROUP BY TxCategoryIdTxt) AS Exp2011,
			  (SELECT SUM(TxAmount) FROM    dbo.Vw_TxCore AS TxCore_1 WHERE    (TxCategoryIdTxt = tc.TxCategoryIdTxt) AND (YEAR(TxDate) = 2012) GROUP BY TxCategoryIdTxt) AS Exp2012,
			  (SELECT SUM(TxAmount) FROM    dbo.Vw_TxCore AS TxCore_1 WHERE    (TxCategoryIdTxt = tc.TxCategoryIdTxt) AND (YEAR(TxDate) = 2013) GROUP BY TxCategoryIdTxt) AS Exp2013,
			  (SELECT SUM(TxAmount) FROM    dbo.Vw_TxCore AS TxCore_1 WHERE    (TxCategoryIdTxt = tc.TxCategoryIdTxt) AND (YEAR(TxDate) = 2014) GROUP BY TxCategoryIdTxt) AS Exp2014,
			  (SELECT SUM(TxAmount) FROM    dbo.Vw_TxCore AS TxCore_1 WHERE    (TxCategoryIdTxt = tc.TxCategoryIdTxt) AND (YEAR(TxDate) = 2015) GROUP BY TxCategoryIdTxt) AS Exp2015,
			  (SELECT SUM(TxAmount) FROM    dbo.Vw_TxCore AS TxCore_1 WHERE    (TxCategoryIdTxt = tc.TxCategoryIdTxt) AND (YEAR(TxDate) = 2016) GROUP BY TxCategoryIdTxt) AS Exp2016,
			  (SELECT SUM(TxAmount) FROM    dbo.Vw_TxCore AS TxCore_1 WHERE    (TxCategoryIdTxt = tc.TxCategoryIdTxt) AND (YEAR(TxDate) = 2017) GROUP BY TxCategoryIdTxt) AS Exp2017,
			  (SELECT SUM(TxAmount) FROM    dbo.Vw_TxCore AS TxCore_1 WHERE    (TxCategoryIdTxt = tc.TxCategoryIdTxt) AND (YEAR(TxDate) = 2018) GROUP BY TxCategoryIdTxt) AS Exp2018,
			  (SELECT SUM(TxAmount) FROM    dbo.Vw_TxCore AS TxCore_1 WHERE    (TxCategoryIdTxt = tc.TxCategoryIdTxt) AND (YEAR(TxDate) = 2019) GROUP BY TxCategoryIdTxt) AS Exp2019,
				(SELECT SUM(TxAmount) FROM    dbo.Vw_TxCore AS TxCore_1 WHERE    (TxCategoryIdTxt = tc.TxCategoryIdTxt) AND (YEAR(TxDate) = 2020) GROUP BY TxCategoryIdTxt) AS Exp2020,
		 
		 (SELECT Max(v) FROM (VALUES 
			 ((SELECT SUM(TxAmount) FROM    dbo.Vw_TxCore AS TxCore_2 WHERE    (TxCategoryIdTxt = tc.TxCategoryIdTxt) AND (YEAR(TxDate) = 2010) GROUP BY TxCategoryIdTxt)),
			 ((SELECT SUM(TxAmount) FROM    dbo.Vw_TxCore AS TxCore_1 WHERE    (TxCategoryIdTxt = tc.TxCategoryIdTxt) AND (YEAR(TxDate) = 2011) GROUP BY TxCategoryIdTxt)),
			 ((SELECT SUM(TxAmount) FROM    dbo.Vw_TxCore AS TxCore_1 WHERE    (TxCategoryIdTxt = tc.TxCategoryIdTxt) AND (YEAR(TxDate) = 2012) GROUP BY TxCategoryIdTxt)),
			 ((SELECT SUM(TxAmount) FROM    dbo.Vw_TxCore AS TxCore_1 WHERE    (TxCategoryIdTxt = tc.TxCategoryIdTxt) AND (YEAR(TxDate) = 2013) GROUP BY TxCategoryIdTxt)),
			 ((SELECT SUM(TxAmount) FROM    dbo.Vw_TxCore AS TxCore_1 WHERE    (TxCategoryIdTxt = tc.TxCategoryIdTxt) AND (YEAR(TxDate) = 2014) GROUP BY TxCategoryIdTxt)),
			 ((SELECT SUM(TxAmount) FROM    dbo.Vw_TxCore AS TxCore_1 WHERE    (TxCategoryIdTxt = tc.TxCategoryIdTxt) AND (YEAR(TxDate) = 2015) GROUP BY TxCategoryIdTxt)),
			 ((SELECT SUM(TxAmount) FROM    dbo.Vw_TxCore AS TxCore_1 WHERE    (TxCategoryIdTxt = tc.TxCategoryIdTxt) AND (YEAR(TxDate) = 2016) GROUP BY TxCategoryIdTxt)),
			 ((SELECT SUM(TxAmount) FROM    dbo.Vw_TxCore AS TxCore_1 WHERE    (TxCategoryIdTxt = tc.TxCategoryIdTxt) AND (YEAR(TxDate) = 2017) GROUP BY TxCategoryIdTxt)),
			 ((SELECT SUM(TxAmount) FROM    dbo.Vw_TxCore AS TxCore_1 WHERE    (TxCategoryIdTxt = tc.TxCategoryIdTxt) AND (YEAR(TxDate) = 2018) GROUP BY TxCategoryIdTxt)),
			 ((SELECT SUM(TxAmount) FROM    dbo.Vw_TxCore AS TxCore_1 WHERE    (TxCategoryIdTxt = tc.TxCategoryIdTxt) AND (YEAR(TxDate) = 2019) GROUP BY TxCategoryIdTxt))) AS value(v)) AS MaxPrev,

    (SELECT SUM(TxAmount) FROM    dbo.Vw_TxCore AS TxCore_1     WHERE    (TxCategoryIdTxt = tc.TxCategoryIdTxt) AND (YEAR(TxDate) = 2020) GROUP BY TxCategoryIdTxt) / (.00000000000000001 + isnull
     ((SELECT Max(v) FROM (VALUES 
			 ((SELECT SUM(TxAmount) FROM    dbo.Vw_TxCore AS TxCore_2 WHERE    (TxCategoryIdTxt = tc.TxCategoryIdTxt) AND (YEAR(TxDate) = 2010) GROUP BY TxCategoryIdTxt)),
			 ((SELECT SUM(TxAmount) FROM    dbo.Vw_TxCore AS TxCore_1 WHERE    (TxCategoryIdTxt = tc.TxCategoryIdTxt) AND (YEAR(TxDate) = 2011) GROUP BY TxCategoryIdTxt)),
			 ((SELECT SUM(TxAmount) FROM    dbo.Vw_TxCore AS TxCore_1 WHERE    (TxCategoryIdTxt = tc.TxCategoryIdTxt) AND (YEAR(TxDate) = 2012) GROUP BY TxCategoryIdTxt)),
			 ((SELECT SUM(TxAmount) FROM    dbo.Vw_TxCore AS TxCore_1 WHERE    (TxCategoryIdTxt = tc.TxCategoryIdTxt) AND (YEAR(TxDate) = 2013) GROUP BY TxCategoryIdTxt)),
			 ((SELECT SUM(TxAmount) FROM    dbo.Vw_TxCore AS TxCore_1 WHERE    (TxCategoryIdTxt = tc.TxCategoryIdTxt) AND (YEAR(TxDate) = 2014) GROUP BY TxCategoryIdTxt)),
			 ((SELECT SUM(TxAmount) FROM    dbo.Vw_TxCore AS TxCore_1 WHERE    (TxCategoryIdTxt = tc.TxCategoryIdTxt) AND (YEAR(TxDate) = 2015) GROUP BY TxCategoryIdTxt)),
			 ((SELECT SUM(TxAmount) FROM    dbo.Vw_TxCore AS TxCore_1 WHERE    (TxCategoryIdTxt = tc.TxCategoryIdTxt) AND (YEAR(TxDate) = 2016) GROUP BY TxCategoryIdTxt)),
			 ((SELECT SUM(TxAmount) FROM    dbo.Vw_TxCore AS TxCore_1 WHERE    (TxCategoryIdTxt = tc.TxCategoryIdTxt) AND (YEAR(TxDate) = 2017) GROUP BY TxCategoryIdTxt)),
			 ((SELECT SUM(TxAmount) FROM    dbo.Vw_TxCore AS TxCore_1 WHERE    (TxCategoryIdTxt = tc.TxCategoryIdTxt) AND (YEAR(TxDate) = 2018) GROUP BY TxCategoryIdTxt)),
			 ((SELECT SUM(TxAmount) FROM    dbo.Vw_TxCore AS TxCore_1 WHERE    (TxCategoryIdTxt = tc.TxCategoryIdTxt) AND (YEAR(TxDate) = 2019) GROUP BY TxCategoryIdTxt))) AS value(v)), 1000)) AS Cur2Max
FROM dbo.Vw_TxCore AS tc INNER JOIN
     dbo.TxCategory   ON tc.TxCategoryIdTxt = dbo.TxCategory.IdTxt INNER JOIN
     dbo.ExpenseGroup ON dbo.TxCategory.ExpGroupId = dbo.ExpenseGroup.Id
GROUP BY dbo.ExpenseGroup.Name, dbo.TxCategory.Name, tc.TxCategoryIdTxt, dbo.ExpenseGroup.Id, dbo.TxCategory.IdTxt, dbo.TxCategory.TL_Number
ORDER BY dbo.ExpenseGroup.Name, TaxLiq##
GO


select * from [Vw_Exp_Hist_vs_Last]