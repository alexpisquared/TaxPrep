use TimeTrackDbRls
go
SELECT (SELECT SUM((TotalHours * RateSubmitted) * (100 + HstPercent) / 100) FROM Invoice WHERE InvoiceeId=27) +(SELECT SUM(x.TxAmount) FROM FinDemo.dbo.TxCoreV2 x WHERE x.TxDetail='ROBCO            PAY' AND x.TxAmount <> 0) [Outsanding Balance], (SELECT CompanyName FROM Invoicee WHERE (Id = 27)) [from Company]

SELECT	i.Id [Outstanding Invoice #],
				FORMAT(PeriodFrom,  'yyyy-MM-dd')		    [Period From], 
				FORMAT(PeriodUpTo,  'yyyy-MM-dd')		    [Period To], 
				FORMAT(InvoiceDate, 'yyyy-MM-dd')		    [Invoiced On], TotalHours,
				DATEDIFF(day, InvoiceDate, getdate())	  InvoicedDaysAgo,
				FORMAT(ScheduledFor,'yyyy-MM-dd · ddd')	[Scheduled For], 
				DATEDIFF(day, ScheduledFor, getdate())  [Days Overdue] 
FROM Invoice i left outer join FinDemo.dbo.TxCoreV2 x ON i.TxCoreV2Id = x.Id 
WHERE (InvoiceeId >= 27) AND (PeriodFrom > CONVERT(DATETIME, '2018-01-01 00:00:00', 102)) AND TxDate is null
ORDER BY PeriodFrom DESC 

SELECT Id [Loose Txn Id], TxDate, TxAmount FROM FinDemo.dbo.TxCoreV2 x WHERE x.TxDetail = 'ROBCO            PAY' AND TxAmount<>0 AND x.Id NOT in (SELECT TxCoreV2Id FROM Invoice WHERE InvoiceeId=27 AND TxCoreV2Id IS NOT NULL)

SELECT COUNT(*) AS InvCnt, SUM((TotalHours * RateSubmitted) * (100 + HstPercent) / 100) AS Ttl	FROM            Invoice	WHERE        (InvoiceeId = 27)
SELECT COUNT(*) AS TxnCnt, SUM(x.TxAmount) AS Ttl, AVG(x.TxAmount) AS Avg FROM            FinDemo.dbo.TxCoreV2 x WHERE        (x.TxDetail = 'ROBCO            PAY') AND (x.TxAmount <> 0)


SELECT  i.Id [Paid Invoice #],
				FORMAT(PeriodFrom,  'yyyy-MM-dd')       [Period From], 
				FORMAT(PeriodUpTo,  'yyyy-MM-dd')       [Period To], 
				FORMAT(InvoiceDate, 'yyyy-MM-dd')       [Invoiced On],         TotalHours, 
				DATEDIFF(day, InvoiceDate,  x.TxDate)   InvoiceTxnGapDays, 
				FORMAT(x.TxDate,	'yyyy-MM-dd · ddd')   TxDate,
				DATEDIFF(day, isnull(ScheduledFor, DATEADD(DAY,11,InvoiceDate)), isnull(x.TxDate, getdate())) [Days Overdue],  
				case	when x.TxAmount is null then '' 
						when x.TxAmount + TotalHours * RateSubmitted * (100+HstPercent)/100 = 0 then 'OK' 
						else 'ERROR: '+cast((x.TxAmount + TotalHours * RateSubmitted * (100+HstPercent)/100) as varchar(99)) end [Txn-Invoice Match]
FROM Invoice i left outer join FinDemo.dbo.TxCoreV2 x ON i.TxCoreV2Id = x.Id 
WHERE (InvoiceeId = 27) 
--AND (PeriodFrom > CONVERT(DATETIME, '2018-01-01 00:00:00', 102)) 
AND TxDate is NOT null
ORDER BY PeriodFrom DESC 

--SELECT        AVG(DATEDIFF(day, i.InvoiceDate, x.TxDate)) AS InvoicedDaysAgo, AVG(DATEDIFF(day, i.ScheduledFor, x.TxDate)) AS OverDue FROM            Invoice AS i LEFT OUTER JOIN FinDemo.dbo.TxCoreV2 AS x ON i.TxCoreV2Id = x.Id WHERE        (i.PeriodFrom > CONVERT(DATETIME, '2018-01-01 00:00:00', 102)) AND (i.InvoiceeId = 27) AND (x.TxDate IS NOT NULL)

/* 	2018 Payment & Invoice Dates						
									
Oct 26, 2018	ROBCO PAY		12,538.48	$64,331.43

Period Worked			Invoice Due Date:	Payment Date:		Actual Txn: Delay:							  	
Dec 26 - Jan 8			Jan 9,	2017			Fri, Jan 20,2017		
Jan 9 - Jan 22			Jan 23,2017			Fri, Feb 3,2017		
Jan 23 - Feb 5			Feb 6,2017			Fri, Feb 17,2017		
Feb 6 - Feb 19			Feb 20,2017			Fri, Mar 3,2017		
Feb 20 - Mar 5			Mar 6,2017			Fri, Mar 17,2017		
Mar 6 - Mar 19			Mar 20,2017			Fri, Mar 31,2017		
Mar 20 - Apr 2			Apr 3,2017			Fri, Apr 14,2017		
Apr 3 - Apr 16			Apr 17,2017			Fri, Apr 28,2017		
Apr 17 - Apr 30			May 1,2017			Fri, May 12,2017		
May 1 - May 14			May 15,2017			Fri, May 26,2017		
May 15 - May 28			May 29,2017			Fri, Jun 9,2017		
May 29 - Jun 11			Jun 12,2017			Fri, Jun 23,2017		
Jun 12 - Jun 25			Jun 26,2017			Fri, Jul 7,2017		
Jun 26 - Jul 9			Jul 10,2017			Fri, Jul 21,2017		
Jul 10 - Jul 23			Jul 24,2017			Fri, Aug 4,2017		
Jul 24 - Aug 6			Aug 7,2017			Fri, Aug 18,2017		
Aug 7 - Aug 20			Aug 21,2017			Fri, Sep 1,2017		
Aug 21 - Sep 3			Sep 4,2017			Fri, Sep 15,2017		
Sep 4 - Sep 17			Sep 18,2017			Fri, Sep 29,2017		
Sep 18 - Oct 1			Oct 2,2017			Fri, Oct 13,2017		
Oct 2 - Oct 15			Oct 16,2017			Fri, Oct 27,2017		
Oct 16 - Oct 29			Oct 30,2017			Fri, Nov 10,2017		
Oct 30 - Nov 12			Nov 13,2017			Fri, Nov 24,2017		
Nov 13 - Nov 26			Nov 27,2017			Fri, Dec 8,2017		
Nov 27 - Dec 10			Dec 11,2017			Fri, Dec 22,2017		
Dec 11 - Dec 24			Dec 25,2017			Fri, Jan 5,2018		

Dec 25 - Jan  7			Jan  8,2018			Jan 19,2018		  2018-01-19     0
Jan  8 - Jan 21			Jan 22,2018			Feb  2,2018		  2018-02-07     5
Jan 22 - Feb  4			Feb  5,2018			Feb 16,2018		  2018-02-22     6
Feb  5 - Feb 18			Feb 19,2018			Mar  2,2018		  2018-03-06     4
Feb 19 - Mar  4			Mar  5,2018			Mar 16,2018		  2018-03-23     7
Mar  5 - Mar 18			Mar 19,2018			Mar 30,2018		  2018-04-06     7
Mar 19 - Apr 1			Apr  2,2018			Apr 13,2018		  2018-04-25    12
Apr  2 - Apr 15			Apr 16,2018			Apr 27,2018		  2018-05-11    14
Apr 16 - Apr 29			Apr 30,2018			May 11,2018		  2018-05-18     7
Apr 30 - May 13			May 14,2018			May 25,2018		  2018-06-01     7
May 14 - May 27			May 28,2018			Jun  8,2018		  2018-06-15     7
May 28 - Jun 10			Jun 11,2018			Jun 22,2018		  2018-06-29     7
Jun 11 - Jun 24			Jun 25,2018			Jul  6,2018		  2018-07-18    12
Jun 25 - Jul  8			Jul  9,2018			Jul 20,2018		  2018-07-27     7
Jul  9 - Jul 22			Jul 23,2018			Aug  3,2018		  2018-08-17    14
Jul 23 - Aug  5			Aug  6,2018			Aug 17,2018		  2018-10-19    75
Aug  6 - Aug 19			Aug 20,2018			Aug 31,2018		  
Aug 20 - Sep  2			Sep  3,2018			Sep 14,2018		  
Sep  3 - Sep 16			Sep 17,2018			Sep 28,2018		
Sep 17 - Sep 30			Oct  1,2018			Oct 12,2018		  
Oct  1 - Oct 14			Oct 15,2018			Oct 26,2018		
Oct 15 - Oct 28			Oct 29,2018			Nov  9,2018		
Oct 29 - Nov 11			Nov 12,2018			Nov 23,2018		
Nov 12 - Nov 25			Nov 26,2018			Dec  7,2018		
Nov 26 - Dec  9			Dec 10,2018			Dec 21,2018		
Dec 10 - Dec 23			Dec 24,2018			Jan  4,2019		


	2018 Payment & Invoice Dates														2017 Payment & Invoice Dates									
																								
Period Worked			Invoice Due:		Payment Date:								Period Worked			Invoice Due:		Payment Date:				
																								
Dec 25 - Jan 7			08-Jan-18			19-Jan-18				11.0				Dec 26 - Jan 8			09-Jan-17			20-Jan-17				11.0
Jan 8 - Jan 21			22-Jan-18			02-Feb-18				11.0				Jan 9 - Jan 22			23-Jan-17			03-Feb-17				11.0
Jan 22 - Feb 4			05-Feb-18			16-Feb-18				11.0				Jan 23 - Feb 5			06-Feb-17			17-Feb-17				11.0
Feb 5 - Feb 18			19-Feb-18			02-Mar-18				11.0				Feb 6 - Feb 19			20-Feb-17			03-Mar-17				11.0
Feb 19 -Mar 4			05-Mar-18			16-Mar-18				11.0				Feb 20 - Mar 5			06-Mar-17			17-Mar-17				11.0
Mar 5 - Mar 18			19-Mar-18			30-Mar-18				11.0				Mar 6 - Mar 19			20-Mar-17			31-Mar-17				11.0
Mar 19 - Apr 1			02-Apr-18			13-Apr-18				11.0				Mar 20 - Apr 2			03-Apr-17			14-Apr-17				11.0
Apr 2 - Apr 15			16-Apr-18			27-Apr-18				11.0				Apr 3 - Apr 16			17-Apr-17			28-Apr-17				11.0
Apr 16 - Apr 29			30-Apr-18			11-May-18				11.0				Apr 17 - Apr 30			01-May-17			12-May-17				11.0
Apr 30 - May 13			14-May-18			25-May-18				11.0				May 1 - May 14			15-May-17			26-May-17				11.0
May 14 - May 27			28-May-18			08-Jun-18				11.0				May 15 - May 28			29-May-17			09-Jun-17				11.0
May 28 - Jun 10			11-Jun-18			22-Jun-18				11.0				May 29 - Jun 11			12-Jun-17			23-Jun-17				11.0
Jun 11 - Jun 24			25-Jun-18			06-Jul-18				11.0				Jun 12 - Jun 25			26-Jun-17			07-Jul-17				11.0
Jun 25 - Jul 8			09-Jul-18			20-Jul-18				11.0				Jun 26 - Jul 9			10-Jul-17			21-Jul-17				11.0
Jul 9 - Jul 22			23-Jul-18			03-Aug-18				11.0				Jul 10 - Jul 23			24-Jul-17			04-Aug-17				11.0
Jul 23 -Aug 5			06-Aug-18			17-Aug-18				11.0				Jul 24 - Aug 6			07-Aug-17			18-Aug-17				11.0
Aug 6 - Aug 19			20-Aug-18			31-Aug-18				11.0				Aug 7 - Aug 20			21-Aug-17			01-Sep-17				11.0
Aug 20 - Sep 2			03-Sep-18			14-Sep-18				11.0				Aug 21 - Sep 3			04-Sep-17			15-Sep-17				11.0
Sep 3 - Sep 16			17-Sep-18			28-Sep-18				11.0				Sep 4 - Sep 17			18-Sep-17			29-Sep-17				11.0
Sep 17 -Sep 30			01-Oct-18			12-Oct-18				11.0				Sep 18 - Oct 1			02-Oct-17			13-Oct-17				11.0
Oct 1 - Oct 14			15-Oct-18			26-Oct-18				11.0				Oct 2 - Oct 15			16-Oct-17			27-Oct-17				11.0
Oct 15 - Oct 28			29-Oct-18			09-Nov-18				11.0				Oct 16 - Oct 29			30-Oct-17			10-Nov-17				11.0
Oct 29 - Nov 11			12-Nov-18			23-Nov-18				11.0				Oct 30 - Nov 12			13-Nov-17			24-Nov-17				11.0
Nov 12 - Nov 25			26-Nov-18			07-Dec-18				11.0				Nov 13 - Nov 26			27-Nov-17			08-Dec-17				11.0
Nov 26 -  Dec 9			10-Dec-18			21-Dec-18				11.0				Nov 27 - Dec 10			11-Dec-17			22-Dec-17				11.0
Dec 10 - Dec 23			24-Dec-18			04-Jan-19				11.0				Dec 11 - Dec 24			25-Dec-17			05-Jan-18				11.0
																								
																								
Invoices must be received prior to the end of business day on the Invoice due date stated above.																								
																								
They must include the following:																								
1. An invoice for the gross amount (including company address, hours, rate and all applicable taxes)																								
2. A supporting record of hours authorized by a designated client contact.																								
3. Any authorized expenses for the period with supporting documentation 																								
	(expenses must be itemized, including copy of receipt, and be authorized by designated client contact)																							
Failure to provide Invoice with supporting documents by the due date may cause a delay in payment.																								
*/