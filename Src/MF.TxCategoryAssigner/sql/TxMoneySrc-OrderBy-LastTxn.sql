
-- A convenient view of accounts sorted by last Txn captured:

SELECT   TxMoneySrc.Owner, TxMoneySrc.Fla, TxMoneySrc.Name, TxMoneySrc.Notes, COUNT(*) AS TtlTxn, MAX(TxCoreV2.TxDate) AS LastTxn
FROM      TxMoneySrc INNER JOIN
                TxCoreV2 ON TxMoneySrc.Id = TxCoreV2.TxMoneySrcId
GROUP BY TxMoneySrc.Owner, TxMoneySrc.Fla, TxMoneySrc.Name, TxMoneySrc.Notes
ORDER BY LastTxn DESC