-- Feb 2017
use FinDemo
go
-- run as is to see what is outstanding and if looks good, select/run next 4 rows including the commented INSERT...:
-- insert into dbo.TxCoreV2 (TxMoneySrcId, TxCategoryIdTxt, TxAmount, TxDate,                TxDetail,               Notes,         MemoPP, CreatedAt, ModifiedAt, FitId)
SELECT                      TxMoneySrcId, TxCategoryIdTxt, TxAmount, TxDate, ProductService TxDetail, 'from TxCore' Notes, History MemoPP, CreatedAt, ModifiedAt, convert (varchar(99), TxDate , 126)+'~'+cast(TxAmount as varchar(99)) FROM            dbo.TxCore 
WHERE        (TxDate > CONVERT(DATETIME, '2016-01-01 00:00:00', 102)) 
	AND Id not IN (SELECT        TxCore.Id FROM            TxCore INNER JOIN TxCoreV2 ON TxCore.TxAmount = TxCoreV2.TxAmount AND TxCore.TxDate = TxCoreV2.TxDate)


--SELECT * FROM TxCoreV2 where id in ( SELECT TxCoreV2.Id FROM            TxCore INNER JOIN TxCoreV2 ON TxCoreV2.FitId = (convert (varchar(99), TxCore.TxDate , 126)+'~'+cast(TxCore.TxAmount as varchar(99)) ) ) ORDER BY Id DESC
--SELECT top (30) * FROM TxCoreV2 order by id desc
