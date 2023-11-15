USE MinFinInv
go

SELECT     AcntHist.InvAccountID, COUNT(*) AS Cnt, MAX(AcntHist.Balance) AS Max, MIN(AcntHist.Balance) AS Min, InvAccount.SrcAccount, InvAccount.TrgAccount, InvAccount.OpenedOn, InvAccount.ClosedOn, InvAccount.BalanceOpening, InvAccount.BalanceClosing, InvAccount.Institution, 
                  InvAccount.AcntType, InvAccount.AcntHolder, InvAccount.Advisor, InvAccount.Notes
FROM        AcntHist INNER JOIN
                  InvAccount ON AcntHist.InvAccountID = InvAccount.ID
WHERE     (AcntHist.Balance <> 0)
GROUP BY AcntHist.InvAccountID, InvAccount.SrcAccount, InvAccount.TrgAccount, InvAccount.OpenedOn, InvAccount.ClosedOn, InvAccount.BalanceOpening, InvAccount.BalanceClosing, InvAccount.Institution, InvAccount.AcntType, InvAccount.AcntHolder, InvAccount.Advisor, InvAccount.Notes
ORDER BY Max DESC

select sum(max) as TtlMax, sum(min) as TtlMin from (
SELECT     AcntHist.InvAccountID, COUNT(*) AS Cnt, MAX(AcntHist.Balance) AS Max, MIN(AcntHist.Balance) AS Min, InvAccount.SrcAccount, InvAccount.TrgAccount, InvAccount.OpenedOn, InvAccount.ClosedOn, InvAccount.BalanceOpening, InvAccount.BalanceClosing, InvAccount.Institution, 
                  InvAccount.AcntType, InvAccount.AcntHolder, InvAccount.Advisor, InvAccount.Notes
FROM        AcntHist INNER JOIN
                  InvAccount ON AcntHist.InvAccountID = InvAccount.ID
WHERE     (AcntHist.Balance <> 0)
GROUP BY AcntHist.InvAccountID, InvAccount.SrcAccount, InvAccount.TrgAccount, InvAccount.OpenedOn, InvAccount.ClosedOn, InvAccount.BalanceOpening, InvAccount.BalanceClosing, InvAccount.Institution, InvAccount.AcntType, InvAccount.AcntHolder, InvAccount.Advisor, InvAccount.Notes
) as kljl