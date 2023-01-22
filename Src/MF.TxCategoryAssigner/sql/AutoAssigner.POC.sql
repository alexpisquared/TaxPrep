USE FinDemoDbg
GO
/*
--USE FinDemoDbg
--GO
--SELECT count(*) FROM TxCoreV2 WHERE                                 (TxDate >= CONVERT(DATETIME, '2022-01-01 00:00:00', 102))
--SELECT count(*) FROM TxCoreV2 WHERE  (TxCategoryIdTxt = 'UnKn') AND (TxDate >= CONVERT(DATETIME, '2022-01-01 00:00:00', 102))
--UPDATE               TxCoreV2 SET     TxCategoryIdTxt = 'UnKn' WHERE (TxCategoryIdTxt <> 'UnKn') AND (TxDate >= CONVERT(DATETIME, '2022-01-01 00:00:00', 102))
--SELECT count(*) FROM TxCoreV2 WHERE  (TxCategoryIdTxt = 'UnKn') AND (TxDate >= CONVERT(DATETIME, '2022-01-01 00:00:00', 102))
--* /
--USE FinDemo
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		OlePi
-- Create date: 2023-01-01
-- Description:	...
-- =============================================
CREATE OR ALTER PROCEDURE GroupedTxn 
	@YearOfInt int = 2022, 
	@MatchLen  int = 1
AS
BEGIN
	SET NOCOUNT ON;

	IF (@MatchLen < 1) 
		SET @MatchLen = 1
	
	DECLARE @YOI AS VARCHAR(32) 
	SET @YOI = CAST(@YearOfInt AS CHAR(4)) + '-01-01 00:00:00' -- SELECT @YearOfInt, @MatchLen, @YOI
	
	DECLARE @YoiDate AS DATETIME
	SET     @YoiDate = CONVERT(DATETIME, @YOI, 102)

	SELECT   SUBSTRING(TxDetail, 1, @MatchLen) AS TxDtl8, 
			 (SELECT COUNT(*) FROM TxCoreV2 AS c WHERE (TxDate < @YoiDate) AND c.TxDetail LIKE SUBSTRING(a.TxDetail, 1, @MatchLen) + '%') AS CntPre, 
			 COUNT(*) AS CntYoi, 
			 MIN(TxAmount) AS Min, MAX(TxAmount) AS Max, AVG(TxAmount) AS Avg, SUM(TxAmount) AS Sum, MIN(TxDate) AS FrstTxn, MAX(TxDate) AS LastTxn
	FROM	 TxCoreV2 AS a
	WHERE    (TxDate >= @YoiDate) 
	         AND EXISTS (SELECT * FROM TxCoreV2 AS b WHERE (TxDate < @YoiDate) AND b.TxDetail LIKE SUBSTRING(a.TxDetail, 1, @MatchLen) + '%') 
	GROUP BY SUBSTRING(TxDetail, 1, @MatchLen), TxCategoryIdTxt
	HAVING   (TxCategoryIdTxt = 'UnKn')
END
GO
*/
--SELECT   SUBSTRING(TxDetail, 1, 4) AS TxDtl8, COUNT(*) AS Cnt, MIN(TxAmount) AS Min$, MAX(TxAmount) AS Max$, AVG(TxAmount) AS Avg, SUM(TxAmount) AS Sum, MIN(TxDate) AS FrstTxn, MAX(TxDate) AS LastTxn FROM        TxCoreV2 WHERE     (TxDate >= CONVERT(DATETIME, '2022-01-01 00:00:00', 102))
--GROUP BY SUBSTRING(TxDetail, 1, 4), TxCategoryIdTxt
--HAVING   (TxCategoryIdTxt = 'UnKn')

exec GroupedTxn 2022, 33
USE FinDemo
go
exec GroupedTxn 2022, 33
