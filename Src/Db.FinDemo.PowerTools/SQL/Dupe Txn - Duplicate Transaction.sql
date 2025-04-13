use FinDemo
go

SELECT     COUNT(*) AS DuplicateRowCount, TxCoreV2.TxDate, TxCoreV2.TxAmount, TxCoreV2.TxDetail, TxCoreV2.MemoPP, TxCoreV2.TxCategoryIdTxt, TxCoreV2.CreatedAt, TxCoreV2.ModifiedAt, TxMoneySrc.Fla, TxMoneySrc.Name, TxMoneySrc.Owner, TxMoneySrc.Notes
FROM        TxCoreV2 INNER JOIN
                  TxMoneySrc ON TxCoreV2.TxMoneySrcId = TxMoneySrc.Id
GROUP BY TxCoreV2.TxDate, TxCoreV2.TxAmount, TxCoreV2.TxDetail, TxCoreV2.MemoPP, TxCoreV2.TxCategoryIdTxt, TxCoreV2.CreatedAt, TxCoreV2.ModifiedAt, TxMoneySrc.Fla, TxMoneySrc.Name, TxMoneySrc.Owner, TxMoneySrc.Notes
HAVING     (COUNT(*) > 1) AND (ABS(TxCoreV2.TxAmount) > 1000)
ORDER BY TxCoreV2.TxDate DESC
go
SELECT     TxCoreV2.Id, TxCoreV2.FitId, TxCoreV2.TxDate, TxCoreV2.TxAmount, TxCoreV2.TxDetail, TxCoreV2.MemoPP, TxCoreV2.Notes, TxCoreV2.TxMoneySrcId, TxCoreV2.TxCategoryIdTxt, TxCoreV2.CreatedAt, TxCoreV2.ModifiedAt, TxMoneySrc.Fla, TxMoneySrc.Name, TxMoneySrc.Owner, 
                  TxMoneySrc.Notes AS Expr1
FROM        TxCoreV2 INNER JOIN
                      (SELECT     TxDate, TxAmount, TxDetail, MemoPP, TxCategoryIdTxt, CreatedAt, ModifiedAt, TxMoneySrcId
                       FROM        TxCoreV2 AS TxCoreV2_1
                       GROUP BY TxDate, TxAmount, TxDetail, MemoPP, TxCategoryIdTxt, CreatedAt, ModifiedAt, TxMoneySrcId
                       HAVING     (COUNT(*) > 1) AND (ABS(TxAmount) > 1000)) AS DuplicateTxns ON TxCoreV2.TxDate = DuplicateTxns.TxDate AND TxCoreV2.TxAmount = DuplicateTxns.TxAmount AND TxCoreV2.TxDetail = DuplicateTxns.TxDetail AND TxCoreV2.MemoPP = DuplicateTxns.MemoPP AND 
                  TxCoreV2.TxCategoryIdTxt = DuplicateTxns.TxCategoryIdTxt AND TxCoreV2.CreatedAt = DuplicateTxns.CreatedAt AND TxCoreV2.ModifiedAt = DuplicateTxns.ModifiedAt AND TxCoreV2.TxMoneySrcId = DuplicateTxns.TxMoneySrcId INNER JOIN
                  TxMoneySrc ON TxCoreV2.TxMoneySrcId = TxMoneySrc.Id
ORDER BY TxCoreV2.TxDate DESC