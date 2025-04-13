

!!!NOT VALIDATED!!!

TODO: compare WITH excel IFtime permits.


USE [FinDemo]
go
--SELECT c.*, TxCategory.Name AS Ctg FROM TxCoreV2 c INNER JOIN TxCategory ON c.TxCategoryIdTxt = TxCategory.IdTxt WHERE (c.TxMoneySrcId = 3) AND (c.TxAmount < 0) AND (c.TxCategoryIdTxt <> 'BankFee') AND (c.TxDate >= CONVERT(DATETIME, '2016-01-01 00:00:00', 102)) AND (c.TxDate < CONVERT(DATETIME, '2016-04-01 00:00:00', 102)) ORDER BY c.TxDate DESC
declare @q0 as DATETIME, @q1 as DATETIME, @q2 as DATETIME, @q3 as DATETIME, @q4 as DATETIME; 

select @q0 = CONVERT(DATETIME, '2016-01-07 00:00:00', 102), -- needs corretion to the actual cut off
@q1 = CONVERT(DATETIME, '2016-04-01 00:00:00', 102), @q2 = CONVERT(DATETIME, '2016-07-01 00:00:00', 102), @q3 = CONVERT(DATETIME, '2016-10-01 00:00:00', 102), @q4 = CONVERT(DATETIME, '2017-01-07 00:00:00', 102)

-- SELECT *																	FROM TxCoreV2 WHERE (TxMoneySrcId = 3) AND (TxAmount < 0) AND TxDetail != 'ACCT BAL REBATE' AND (TxDate >= @q0) AND (TxDate < @q1) ORDER BY TxDate 
SELECT SUM(TxAmount) as [Revenue 1/4], SUM(TxAmount)/1.13*.088 + 300 as HST FROM TxCoreV2 WHERE (TxMoneySrcId = 3) AND (TxAmount < 0) AND TxDetail != 'ACCT BAL REBATE' AND (TxDate >= @q0) AND (TxDate < @q1) -- +300 for the first quarter only!!!

-- SELECT *															  FROM TxCoreV2 WHERE (TxMoneySrcId = 3) AND (TxAmount < 0) AND TxDetail != 'ACCT BAL REBATE' AND (TxDate >= @q1) AND (TxDate < @q2) ORDER BY TxDate 
SELECT SUM(TxAmount) as [Revenue 2/4], SUM(TxAmount)/1.13*.088 as HST FROM TxCoreV2 WHERE (TxMoneySrcId = 3) AND (TxAmount < 0) AND TxDetail != 'ACCT BAL REBATE' AND (TxDate >= @q1) AND (TxDate < @q2) 

-- SELECT *															  FROM TxCoreV2 WHERE (TxMoneySrcId = 3) AND (TxAmount < 0) AND TxDetail != 'ACCT BAL REBATE' AND (TxDate >= @q2) AND (TxDate < @q3) ORDER BY TxDate 
SELECT SUM(TxAmount) as [Revenue 3/4], SUM(TxAmount)/1.13*.088 as HST FROM TxCoreV2 WHERE (TxMoneySrcId = 3) AND (TxAmount < 0) AND TxDetail != 'ACCT BAL REBATE' AND (TxDate >= @q2) AND (TxDate < @q3) 

-- SELECT *															  FROM TxCoreV2 WHERE (TxMoneySrcId = 3) AND (TxAmount < 0) AND TxDetail != 'ACCT BAL REBATE' AND (TxDate >= @q3) AND (TxDate < @q4) ORDER BY TxDate 
SELECT SUM(TxAmount) as [Revenue 4/4], SUM(TxAmount)/1.13*.088 as HST FROM TxCoreV2 WHERE (TxMoneySrcId = 3) AND (TxAmount < 0) AND TxDetail != 'ACCT BAL REBATE' AND (TxDate >= @q3) AND (TxDate < @q4) 

print '**** Total for the year ****'

-- SELECT *																			  FROM TxCoreV2 WHERE (TxMoneySrcId = 3) AND (TxAmount < 0) AND TxDetail != 'ACCT BAL REBATE' AND (TxDate >= @q0) AND (TxDate < @q4) ORDER BY TxDate 
SELECT SUM(TxAmount) as [Anual Revenue], 
SUM(TxAmount)/1.13*.088 + 300 as [HST WRONG], SUM(TxAmount)     *.088 + 300 as [HST RIGHT],
SUM(TxAmount)/1.13*.088 + 300 - (SUM(TxAmount)     *.088 + 300) as [To Pay !!!] 
FROM TxCoreV2 WHERE (TxMoneySrcId = 3) AND (TxAmount < 0) AND TxDetail != 'ACCT BAL REBATE' AND (TxDate >= @q0) AND (TxDate < @q4) -- +300 for the first quarter only!!!

--SELECT *				  FROM TxCoreV2 WHERE (TxMoneySrcId = 3) AND (TxAmount < 0) AND TxDetail != 'ACCT BAL REBATE' AND (TxDate >= @q0) AND (TxDate < @q4) and TxDetail  = 'ROBCO            PAY' ORDER BY TxDate 
SELECT SUM(TxAmount) [IT] FROM TxCoreV2 WHERE (TxMoneySrcId = 3) AND (TxAmount < 0) AND TxDetail != 'ACCT BAL REBATE' AND (TxDate >= @q0) AND (TxDate < @q4) and TxDetail  = 'ROBCO            PAY' 

--SELECT *				  FROM TxCoreV2 WHERE (TxMoneySrcId = 3) AND (TxAmount < 0) AND TxDetail != 'ACCT BAL REBATE' AND (TxDate >= @q0) AND (TxDate < @q4) and TxDetail != 'ROBCO            PAY' and TxDetail != 'ACCT BAL REBATE' ORDER BY TxDate 
SELECT SUM(TxAmount) [RE] FROM TxCoreV2 WHERE (TxMoneySrcId = 3) AND (TxAmount < 0) AND TxDetail != 'ACCT BAL REBATE' AND (TxDate >= @q0) AND (TxDate < @q4) and TxDetail != 'ROBCO            PAY' and TxDetail != 'ACCT BAL REBATE' 

