SELECT        TOP (100) PERCENT dbo.TxMoneySrc.Owner, dbo.TxMoneySrc.Fla, dbo.TxMoneySrc.Name, dbo.TxMoneySrc.Notes, COUNT(*) AS TxCount, MAX(dbo.TxCoreV2.TxDate) AS LastTx
FROM            dbo.TxMoneySrc INNER JOIN
                         dbo.TxCoreV2 ON dbo.TxMoneySrc.Id = dbo.TxCoreV2.TxMoneySrcId
GROUP BY dbo.TxMoneySrc.Owner, dbo.TxMoneySrc.Fla, dbo.TxMoneySrc.Name, dbo.TxMoneySrc.Notes
ORDER BY LastTx DESC