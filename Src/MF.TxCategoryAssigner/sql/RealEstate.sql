use FinDemo
go
/*SELECT COUNT(*) AS Count, SUM(TxCoreV2.TxAmount) AS Total, TxCoreV2.TxDetail, TxCoreV2.MemoPP, TxMoneySrc.Owner, TxMoneySrc.Fla, TxMoneySrc.Name, TxMoneySrc.Notes AS Expr1
FROM TxCoreV2 INNER JOIN TxMoneySrc ON TxCoreV2.TxMoneySrcId = TxMoneySrc.Id
WHERE (YEAR(TxCoreV2.TxDate) = @Yr) AND (TxCoreV2.TxDetail LIKE '%scc %')
GROUP BY TxCoreV2.TxDetail, TxCoreV2.MemoPP, TxMoneySrc.Owner, TxMoneySrc.Fla, TxMoneySrc.Name, TxMoneySrc.Notes

--SELECT Id, FitId, TxDate, TxAmount, TxDetail, MemoPP, Notes, TxMoneySrcId, TxCategoryIdTxt, CreatedAt, ModifiedAt, CurBalance, HstTakenAt FROM TxCoreV2 WHERE (MemoPP LIKE '%pav%')

SELECT COUNT(*) AS Count, SUM(TxAmount) AS Total FROM TxCoreV2 WHERE YEAR(TxDate)=@Yr and (TxDetail LIKE '%tscc 25%') 
--SELECT * FROM TxCoreV2 WHERE YEAR(TxDate)=@Yr and (TxDetail LIKE '%yrsc%') order by TxDate desc
SELECT COUNT(*) AS Count FROM TxCoreV2 WHERE YEAR(TxDate)=@Yr and (TxDetail LIKE '%- operat%') 

SELECT COUNT(*) AS Count, TxAmount FROM TxCoreV2 WHERE (YEAR(TxDate) = @Yr) AND (TxDetail LIKE '%- operat%') GROUP BY TxAmount HAVING (TxAmount BETWEEN  0 AND 30)
SELECT COUNT(*) AS Count, TxAmount FROM TxCoreV2 WHERE (YEAR(TxDate) = @Yr) AND (TxDetail LIKE '%- operat%') GROUP BY TxAmount HAVING (TxAmount BETWEEN 30 AND 99)
SELECT COUNT(*) AS Count, TxAmount FROM TxCoreV2 WHERE (YEAR(TxDate) = @Yr) AND (TxDetail LIKE '%- operat%') GROUP BY TxAmount HAVING (TxAmount BETWEEN 99 AND 600)

SELECT * FROM TxCoreV2 WHERE YEAR(TxDate)=@Yr and (MemoPP LIKE '%NIAZ%' OR MemoPP LIKE '%JIAQI ZHANG%' ) order by TxDate
SELECT * FROM TxCoreV2 WHERE (TxDetail LIKE 'cash ma%') ORDER BY Id DESC


  SELECT '503 York' as Property, COUNT(*) AS Count, FORMAT(SUM(TxAmount), '#,##0') AS TotalRent, FORMAT(SUM(TxAmount) / 12, '#,##0') AS AvgPerMonth
  FROM TxCoreV2
  WHERE YEAR(TxDate)=@Yr and TxAmount = -2270
UNION ALL
  SELECT '536 South Side', COUNT(*), FORMAT(SUM(TxAmount), '#,##0') , FORMAT(SUM(TxAmount) / 12, '#,##0')
  FROM TxCoreV2
  WHERE YEAR(TxDate)=@Yr and MemoPP LIKE '%vin caesar%'
UNION ALL
  SELECT '1501 Indie', COUNT(*), FORMAT(SUM(TxAmount), '#,##0') , FORMAT(SUM(TxAmount) / 12, '#,##0')
  FROM TxCoreV2
  WHERE YEAR(TxDate)=@Yr and MemoPP LIKE '%zimin%'
UNION ALL
  SELECT '555 Wilson', COUNT(*), FORMAT(SUM(TxAmount), '#,##0') , FORMAT(SUM(TxAmount) / 12, '#,##0')
  FROM TxCoreV2
  WHERE YEAR(TxDate)=@Yr and MemoPP LIKE '%OLANI%'
UNION ALL
  SELECT '1510 Mobilio', COUNT(*), FORMAT(SUM(TxAmount), '#,##0') , FORMAT(SUM(TxAmount) / 12, '#,##0')
  FROM TxCoreV2
  WHERE YEAR(TxDate)=@Yr and (MemoPP LIKE '%DAWOOD%' or MemoPP LIKE '%sierra raposo%')
UNION ALL
  SELECT '610 Pavilia', COUNT(*), FORMAT(SUM(TxAmount) -(-9500 + 2300*2), '#,##0'), FORMAT((SUM(TxAmount) -(-9500 + 2300*2)) / 12, '#,##0')
  FROM TxCoreV2
  WHERE YEAR(TxDate)=@Yr and (MemoPP LIKE '%NIAZ%' OR MemoPP LIKE '%JIAQI ZHANG%')
*/
DECLARE 
@Indie varchar(50) = '1501 Indie',
@Pavil varchar(50) = '610 Pavilia',
@Wilsn varchar(50) = '555 Wilson',
@York_ varchar(50) = '503 York',
@Mobil varchar(50) = '1510 Mobilio',
@South varchar(50) = '536 South Side',
@Yr int = 2024

SELECT @Indie as Property, COUNT(*) AS Count, SUM(TxAmount) AS TotalInsurance, FORMAT(SUM(TxAmount) / 12, '#,##0.##') AS AvgPerMonth  FROM TxCoreV2 WHERE YEAR(TxDate)=@Yr and TxCategoryIdTxt = 'InsSec' and Notes = @Indie UNION ALL
SELECT @Pavil as Property, COUNT(*) AS Count, SUM(TxAmount) AS TotalInsurance, FORMAT(SUM(TxAmount) / 12, '#,##0.##') AS AvgPerMonth  FROM TxCoreV2 WHERE YEAR(TxDate)=@Yr and TxCategoryIdTxt = 'InsSec' and Notes = @Pavil UNION ALL
SELECT @Wilsn as Property, COUNT(*) AS Count, SUM(TxAmount) AS TotalInsurance, FORMAT(SUM(TxAmount) / 12, '#,##0.##') AS AvgPerMonth  FROM TxCoreV2 WHERE YEAR(TxDate)=@Yr and TxCategoryIdTxt = 'InsSec' and Notes = @Wilsn UNION ALL
SELECT @York_ as Property, COUNT(*) AS Count, SUM(TxAmount) AS TotalInsurance, FORMAT(SUM(TxAmount) / 12, '#,##0.##') AS AvgPerMonth  FROM TxCoreV2 WHERE YEAR(TxDate)=@Yr and TxCategoryIdTxt = 'InsSec' and Notes = @York_ UNION ALL
SELECT @Mobil as Property, COUNT(*) AS Count, SUM(TxAmount) AS TotalInsurance, FORMAT(SUM(TxAmount) / 12, '#,##0.##') AS AvgPerMonth  FROM TxCoreV2 WHERE YEAR(TxDate)=@Yr and TxCategoryIdTxt = 'InsSec' and Notes = @Mobil UNION ALL
SELECT @South as Property, COUNT(*) AS Count, SUM(TxAmount) AS TotalInsurance, FORMAT(SUM(TxAmount) / 12, '#,##0.##') AS AvgPerMonth  FROM TxCoreV2 WHERE YEAR(TxDate)=@Yr and TxCategoryIdTxt = 'InsSec' and Notes = @South

-- SELECT * FROM TxCoreV2 WHERE Notes = '555 Wilson' ORDER BY Id DESC 
--SELECT '536 South Side', * FROM TxCoreV2 WHERE YEAR(TxDate)=@Yr and MemoPP LIKE '%vin caesar%'
