use FinDemo
go
/*SELECT COUNT(*) AS Count, SUM(TxCoreV2.TxAmount) AS Total, TxCoreV2.TxDetail, TxCoreV2.MemoPP, TxMoneySrc.Owner, TxMoneySrc.Fla, TxMoneySrc.Name, TxMoneySrc.Notes AS Expr1
FROM TxCoreV2 INNER JOIN TxMoneySrc ON TxCoreV2.TxMoneySrcId = TxMoneySrc.Id
WHERE (YEAR(TxCoreV2.TxDate) = 2024) AND (TxCoreV2.TxDetail LIKE '%scc %')
GROUP BY TxCoreV2.TxDetail, TxCoreV2.MemoPP, TxMoneySrc.Owner, TxMoneySrc.Fla, TxMoneySrc.Name, TxMoneySrc.Notes

--SELECT Id, FitId, TxDate, TxAmount, TxDetail, MemoPP, Notes, TxMoneySrcId, TxCategoryIdTxt, CreatedAt, ModifiedAt, CurBalance, HstTakenAt FROM TxCoreV2 WHERE (MemoPP LIKE '%pav%')

SELECT COUNT(*) AS Count, SUM(TxAmount) AS Total FROM TxCoreV2 WHERE YEAR(TxDate)=2024 and (TxDetail LIKE '%tscc 25%') 
--SELECT * FROM TxCoreV2 WHERE YEAR(TxDate)=2024 and (TxDetail LIKE '%yrsc%') order by TxDate desc
SELECT COUNT(*) AS Count FROM TxCoreV2 WHERE YEAR(TxDate)=2024 and (TxDetail LIKE '%- operat%') 

SELECT COUNT(*) AS Count, TxAmount FROM TxCoreV2 WHERE (YEAR(TxDate) = 2024) AND (TxDetail LIKE '%- operat%') GROUP BY TxAmount HAVING (TxAmount BETWEEN  0 AND 30)
SELECT COUNT(*) AS Count, TxAmount FROM TxCoreV2 WHERE (YEAR(TxDate) = 2024) AND (TxDetail LIKE '%- operat%') GROUP BY TxAmount HAVING (TxAmount BETWEEN 30 AND 99)
SELECT COUNT(*) AS Count, TxAmount FROM TxCoreV2 WHERE (YEAR(TxDate) = 2024) AND (TxDetail LIKE '%- operat%') GROUP BY TxAmount HAVING (TxAmount BETWEEN 99 AND 600)

SELECT * FROM TxCoreV2 WHERE YEAR(TxDate)=2024 and (MemoPP LIKE '%NIAZ%' OR MemoPP LIKE '%JIAQI ZHANG%' ) order by TxDate
SELECT * FROM TxCoreV2 WHERE (TxDetail LIKE 'cash ma%') ORDER BY Id DESC
*/

  SELECT
    '503 York condo' as Property,
    COUNT(*) AS Count,
    SUM(TxAmount) AS TotalRent, SUM(TxAmount) / 12 AS AvgPerMonth
  FROM TxCoreV2
  WHERE YEAR(TxDate)=2024 and TxAmount = -2270
UNION ALL
  SELECT
    '536 South Side',
    COUNT(*),
    SUM(TxAmount), SUM(TxAmount) / 12 AS AvgPerMonth
  FROM TxCoreV2
  WHERE YEAR(TxDate)=2024 and MemoPP LIKE '%vin caesar%'
UNION ALL
  SELECT
    '1501 Indie',
    COUNT(*),
    SUM(TxAmount), SUM(TxAmount) / 12 AS AvgPerMonth
  FROM TxCoreV2
  WHERE YEAR(TxDate)=2024 and MemoPP LIKE '%zimin%'
UNION ALL
  SELECT
    '505 Wilson',
    COUNT(*),
    SUM(TxAmount), SUM(TxAmount) / 12 AS AvgPerMonth
  FROM TxCoreV2
  WHERE YEAR(TxDate)=2024 and MemoPP LIKE '%OLANI%'
UNION ALL
  SELECT
    '1510 Mobilio',
    COUNT(*),
    SUM(TxAmount), SUM(TxAmount) / 12 AS AvgPerMonth
  FROM TxCoreV2
  WHERE YEAR(TxDate)=2024 and (MemoPP LIKE '%DAWOOD%' or MemoPP LIKE '%sierra raposo%')
UNION ALL
  SELECT
    '610 Pavilia',
    COUNT(*),
    (SUM(TxAmount) -(-9500 + 2300*2)),
    (SUM(TxAmount) -(-9500 + 2300*2)) / 12 AS AvgPerMonth
  FROM TxCoreV2
  WHERE YEAR(TxDate)=2024 and (MemoPP LIKE '%NIAZ%' OR MemoPP LIKE '%JIAQI ZHANG%')
