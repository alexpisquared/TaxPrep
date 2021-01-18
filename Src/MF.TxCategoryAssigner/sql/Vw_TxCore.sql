USE [FinDemo] /****** Object:  View [dbo].[Vw_TxCore]    Script Date: 2021-01-18 10:55:47 ******/
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER VIEW [dbo].[Vw_TxCore]
AS
      SELECT TOP (100) PERCENT 10000+Id as Id, TxMoneySrcId, TxCategoryIdTxt, TxAmount, TxDate,                 TxDetail, TxDetail ProductService, Notes,         MemoPP, MemoPP History,       FitId,      CurBalance, CreatedAt, ModifiedAt, TxAmount TxAmountOrg, null TxCategoryId, null TxSupplierId, null TxEnvelopeId FROM dbo.TxCoreV2	WHERE (TxDate >= CONVERT(DATETIME, '2015-01-01 00:00:00', 102) AND TxMoneySrcId <> 18)
union	SELECT TOP (100) PERCENT             Id, TxMoneySrcId, TxCategoryIdTxt, TxAmount, TxDate,  ProductService TxDetail,          ProductService, Notes, History MemoPP,        History, null  FitId, null CurBalance, CreatedAt, ModifiedAt,          TxAmountOrg,      TxCategoryId,      TxSupplierId,      TxEnvelopeId FROM dbo.TxCore	  WHERE (TxDate  < CONVERT(DATETIME, '2015-01-01 00:00:00', 102))	
order by TxDate DESC
GO
/*
2021-01-18 - removed Old JmPcMc #18 from .. as they all are in #21:   SELECT * FROM TxCoreV2 WHERE (TxMoneySrcId = 18 OR TxMoneySrcId = 21) AND (TxDate >= '2018-01-01') AND (TxDate <= '2018-08-08') ORDER BY TxAmount DESC, TxMoneySrcId, TxDate DESC
*/