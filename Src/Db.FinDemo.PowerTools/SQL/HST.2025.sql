SET NOCOUNT ON;
-- SELECT c.*, TxCategory.Name AS Ctg FROM TxCoreV2 c INNER JOIN TxCategory ON c.TxCategoryIdTxt = TxCategory.IdTxt WHERE (c.TxMoneySrcId = 3) AND (c.TxAmount < 0) AND (c.TxCategoryIdTxt <> 'BankFee') AND (c.TxDate >= CONVERT(DATETIME, '2025-01-01 00:00:00', 102)) AND (c.TxDate < CONVERT(DATETIME, '2025-04-01 00:00:00', 102)) ORDER BY c.TxDate DESC
declare @q0 as DATETIME, @q1 as DATETIME, @q2 as DATETIME, @q3 as DATETIME, @q4 as DATETIME, @abr as CHAR(32), @a as MONEY, @d as MONEY; 
select @q0 = CONVERT(DATETIME, '2025-01-01 00:00:00', 102), 
       @q1 = CONVERT(DATETIME, '2025-04-01 00:00:00', 102), 
       @q2 = CONVERT(DATETIME, '2025-07-01 00:00:00', 102), 
       @q3 = CONVERT(DATETIME, '2025-10-01 00:00:00', 102), 
       @q4 = CONVERT(DATETIME, '2026-01-01 00:00:00', 102), @abr = 'ACCT BAL REBATE'
	   , @a = 0 -- 85*7.5*10 -- move 2nd to 1st quarter
	   , @d = 0 -- 85*7.5*2  -- move 3rd to 4th quarter

SELECT *																								FROM TxCoreV2 WHERE (TxMoneySrcId = 3) AND (TxAmount < 0) AND TxDetail != @abr AND (TxDate >= @q0) AND (TxDate < @q1) ORDER BY TxDate 
SELECT -isnull(SUM(TxAmount),0)+@a	as [Revenue 1/4], (-isnull(SUM(TxAmount),0)+@a)*.088 - 300	as HST	FROM TxCoreV2 WHERE (TxMoneySrcId = 3) AND (TxAmount < 0) AND TxDetail != @abr AND (TxDate >= @q0) AND (TxDate < @q1)                 -- +300 for the first quarter only!!!

SELECT *																								FROM TxCoreV2 WHERE (TxMoneySrcId = 3) AND (TxAmount < 0) AND TxDetail != @abr AND (TxDate >= @q1) AND (TxDate < @q2) ORDER BY TxDate 
SELECT -isnull(SUM(TxAmount),0)-@a	as [Revenue 2/4], (-isnull(SUM(TxAmount),0)-@a)*.088		as HST	FROM TxCoreV2 WHERE (TxMoneySrcId = 3) AND (TxAmount < 0) AND TxDetail != @abr AND (TxDate >= @q1) AND (TxDate < @q2) 

SELECT *																								FROM TxCoreV2 WHERE (TxMoneySrcId = 3) AND (TxAmount < 0) AND TxDetail != @abr AND (TxDate >= @q2) AND (TxDate < @q3) ORDER BY TxDate 
SELECT -isnull(SUM(TxAmount),0)-@d	as [Revenue 3/4], (-isnull(SUM(TxAmount),0)-@d)*.088		as HST	FROM TxCoreV2 WHERE (TxMoneySrcId = 3) AND (TxAmount < 0) AND TxDetail != @abr AND (TxDate >= @q2) AND (TxDate < @q3) 

SELECT *					   															                FROM TxCoreV2 WHERE (TxMoneySrcId = 3) AND (TxAmount < 0) AND TxDetail != @abr AND (TxDate >= @q3) AND (TxDate < @q4) ORDER BY TxDate 
SELECT -isnull(SUM(TxAmount),0)+@d	as [Revenue 4/4], (-isnull(SUM(TxAmount),0)+@d)*.088		as HST	FROM TxCoreV2 WHERE (TxMoneySrcId = 3) AND (TxAmount < 0) AND TxDetail != @abr AND (TxDate >= @q3) AND (TxDate < @q4) 

SELECT -SUM(TxAmount) as [■ ■ Anual Revenue 101], FORMAT(ROUND(SUM(TxAmount)*-.088, 2), '0.##')	as [HST NetFile 105], 300 as [HST NetFile 108], FORMAT(ROUND(SUM(TxAmount)*-.088 - 300, 2), '0.##') as [HST 110], SUM(TxAmount) *-.088 - 300 as UnRounded
													                                               FROM TxCoreV2 WHERE (TxMoneySrcId = 3) AND (TxAmount < 0) AND TxDetail != @abr AND (TxDate >= @q0) AND (TxDate < @q4) 
--SELECT *                FROM TxCoreV2 WHERE (TxMoneySrcId = 3) AND (TxAmount < 0) AND TxDetail != @abr AND (TxDate >= @q0) AND (TxDate < @q4) and TxDetail  = 'IRESS MKT TECH   BPY' ORDER BY TxDate 
--SELECT SUM(TxAmount) [IT] FROM TxCoreV2 WHERE (TxMoneySrcId = 3) AND (TxAmount < 0) AND TxDetail != @abr AND (TxDate >= @q0) AND (TxDate < @q4) and TxDetail  = 'IRESS MKT TECH   BPY' 
--SELECT *                FROM TxCoreV2 WHERE (TxMoneySrcId = 3) AND (TxAmount < 0) AND TxDetail != @abr AND (TxDate >= @q0) AND (TxDate < @q4) and TxDetail != 'IRESS MKT TECH   BPY' and TxDetail != @abr ORDER BY TxDate 
--SELECT SUM(TxAmount) [RE] FROM TxCoreV2 WHERE (TxMoneySrcId = 3) AND (TxAmount < 0) AND TxDetail != @abr AND (TxDate >= @q0) AND (TxDate < @q4) and TxDetail != 'IRESS MKT TECH   BPY' and TxDetail != @abr 


