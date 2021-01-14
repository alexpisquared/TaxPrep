-- Editable(!) dupe list:
SELECT        Id, FitId, TxDate, TxDetail, CreatedAt, TxAmount, Notes,
                             (SELECT        Name
                               FROM            TxMoneySrc
                               WHERE        (Id = T1.TxMoneySrcId)) AS TxMoneySrc
FROM            TxCoreV2 AS T1
WHERE        (TxDate IN
                             (SELECT        TOP (888) TxDate
                               FROM            TxCoreV2
                               WHERE        (CreatedAt > CONVERT(DATETIME, '2016-01-10 11:00:00', 102))
                               GROUP BY TxDate, TxAmount
                               HAVING         (COUNT(*) > 1))) AND (TxAmount IN
                             (SELECT        TOP (888) TxAmount
                               FROM            TxCoreV2 AS TxCoreV2_1
                               WHERE        (CreatedAt > CONVERT(DATETIME, '2016-01-10 11:00:00', 102))
                               GROUP BY TxDate, TxAmount
                               HAVING         (COUNT(*) > 1)))
ORDER BY TxDate DESC, TxAmount DESC, CreatedAt DESC

-- Non-editable but with the count:
SELECT         T1.Id, T1.FitId, T1.TxDate, T1.TxDetail, T1.CreatedAt, T1.TxAmount, T1.Notes, TxMoneySrc.Name TxMoneySrc, t0.DupeCnt
FROM            (SELECT        TOP (888) COUNT(*) AS DupeCnt, TxDate, TxAmount
                          FROM            TxCoreV2
                          WHERE        (CreatedAt > CONVERT(DATETIME, '2016-01-10 11:00:00', 102))
                          GROUP BY TxDate, TxAmount
                          HAVING         (COUNT(*) > 1)) AS t0 INNER JOIN
                         TxCoreV2 AS T1 ON t0.TxDate = T1.TxDate AND t0.TxAmount = T1.TxAmount INNER JOIN
                         TxMoneySrc ON T1.TxMoneySrcId = TxMoneySrc.Id
ORDER BY T1.TxDate DESC, T1.TxAmount DESC, T1.CreatedAt DESC


