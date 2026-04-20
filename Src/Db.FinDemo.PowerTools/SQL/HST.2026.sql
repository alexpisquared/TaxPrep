SET NOCOUNT ON;
-- SELECT c.*, TxCategory.Name AS Ctg FROM TxCoreV2 c INNER JOIN TxCategory ON c.TxCategoryIdTxt = TxCategory.IdTxt WHERE (c.TxMoneySrcId = 3) AND (c.TxAmount < 0) AND (c.TxCategoryIdTxt <> 'BankFee') AND (c.TxDate >= CONVERT(DATETIME, '2026-01-01 00:00:00', 102)) AND (c.TxDate < CONVERT(DATETIME, '2026-04-01 00:00:00', 102)) ORDER BY c.TxDate DESC
declare @q0 as DATETIME, @q1 as DATETIME, @q2 as DATETIME, @q3 as DATETIME, @q4 as DATETIME, @abr as CHAR(32), @a as MONEY, @d as MONEY; 
select @q0 = CONVERT(DATETIME, '2026-01-01 00:00:00', 102), 
       @q1 = CONVERT(DATETIME, '2026-04-01 00:00:00', 102), 
       @q2 = CONVERT(DATETIME, '2026-07-01 00:00:00', 102), 
       @q3 = CONVERT(DATETIME, '2026-10-01 00:00:00', 102), 
       @q4 = CONVERT(DATETIME, '2027-01-01 00:00:00', 102), @abr = 'ACCT BAL REBATE'

SELECT *																								FROM TxCoreV2 WHERE (TxMoneySrcId = 3) AND (TxAmount < 0) AND TxDetail != @abr AND (TxDate >= @q0) AND (TxDate < @q1) ORDER BY TxDate 
SELECT -isnull(SUM(TxAmount)/4,0)	as [Revenue 1/4], (-isnull(SUM(TxAmount)/4,0))*.088-300 as HST	FROM TxCoreV2 WHERE (TxMoneySrcId = 3) AND (TxAmount < 0) AND (TxDate >= @q0) AND (TxDate < @q1)                 -- +300 for the first quarter only!!!
SELECT -isnull(SUM(TxAmount)/4,0)	as [Revenue 2/4], (-isnull(SUM(TxAmount)/4,0))*.088     as HST	FROM TxCoreV2 WHERE (TxMoneySrcId = 3) AND (TxAmount < 0) AND (TxDate >= @q0) AND (TxDate < @q1)                 
SELECT -isnull(SUM(TxAmount)/4,0)	as [Revenue 3/4], (-isnull(SUM(TxAmount)/4,0))*.088	    as HST	FROM TxCoreV2 WHERE (TxMoneySrcId = 3) AND (TxAmount < 0) AND (TxDate >= @q0) AND (TxDate < @q1)                 
SELECT -isnull(SUM(TxAmount)/4,0)	as [Revenue 4/4], (-isnull(SUM(TxAmount)/4,0))*.088	    as HST	FROM TxCoreV2 WHERE (TxMoneySrcId = 3) AND (TxAmount < 0) AND (TxDate >= @q0) AND (TxDate < @q1)                 

SELECT -SUM(TxAmount) as [■ ■ Anual Revenue 101], FORMAT(ROUND(SUM(TxAmount)*-.088, 2), '0.##')	as [HST NetFile 105], 300 as [HST NetFile 108], FORMAT(ROUND(SUM(TxAmount)*-.088 - 300, 2), '0.##') as [HST 110], SUM(TxAmount) *-.088 - 300 as UnRounded
													                                               FROM TxCoreV2 WHERE (TxMoneySrcId = 3) AND (TxAmount < 0) AND TxDetail != @abr AND (TxDate >= @q0) AND (TxDate < @q4)              AND Id <> 18321 AND Id <> 16832 -- carbon and tax refunds
