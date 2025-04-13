using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Db.FinDemo7.Models;

namespace MinFin7MdiLib.Services
{
  public class TaxReportService
  {
    public async Task<IEnumerable<VwTaxLiqReport>> GetTaxReportDataAsync(string owner, FinDemoContext dba, DateTime startDate, DateTime endDate)
    {
      try
      {
        // Query using Entity Framework to fetch the report data
        // This matches the SQL query in the requirements
        var reportData = await dba.VwTaxLiqReports
            .Where(x => x.Owner == owner &&
                   x.TlNumber != null &&
                   x.TlNumber <= 201 &&
                   x.FirstTx >= startDate &&
                   x.LastTx <= endDate)
            .OrderBy(x => x.TlNumber)
            .ToListAsync();

        return reportData;
      }
      catch (Exception ex)
      {
        // Log the exception
        Console.WriteLine($"Error in GetTaxReportDataAsync: {ex.Message}");
        throw; // Rethrow to be handled by caller
      }
    }

    // Alternative implementation if you need to use raw SQL instead of EF
    public async Task<IEnumerable<VwTaxLiqReport>> GetTaxReportDataFromRawSqlAsync(string owner, DateTime startDate, DateTime endDate)
    {
      try
      {
        using (var context = new FinDemoContext())
        {
          string startDateStr = startDate.ToString("yyyy-MM-dd 00:00:00");
          string endDateStr = endDate.ToString("yyyy-MM-dd 23:59:59");

          // Using the exact SQL query from the requirements
          var sql = @"
                        SELECT TOP (100) PERCENT 
                            dbo.ExpenseGroup.Name AS [Group], 
                            dbo.TxCategory.TL_Number, 
                            dbo.TxCategory.Name AS Category, 
                            CASE WHEN TxCategory.DeductiblePercentage = 1 THEN '' ELSE CAST(ROUND(SUM(TxCoreV2.TxAmount), 2) AS varchar(9)) + ' * ' + CAST(100 * TxCategory.DeductiblePercentage AS varchar(15)) + '%' END AS PartCalcShow, 
                            ROUND(SUM(dbo.TxCategory.DeductiblePercentage * dbo.TxCoreV2.TxAmount), 2) AS TtlExp, 
                            MIN(dbo.TxCoreV2.TxDate) AS FirstTx, 
                            MAX(dbo.TxCoreV2.TxDate) AS LastTx, 
                            COUNT(*) AS Qnt, 
                            dbo.TxMoneySrc.Owner
                        FROM dbo.ExpenseGroup 
                        INNER JOIN dbo.TxCategory ON dbo.ExpenseGroup.Id = dbo.TxCategory.ExpGroupId 
                        INNER JOIN dbo.TxCoreV2 ON dbo.TxCategory.IdTxt = dbo.TxCoreV2.TxCategoryIdTxt 
                        INNER JOIN dbo.TxMoneySrc ON dbo.TxCoreV2.TxMoneySrcId = dbo.TxMoneySrc.Id
                        WHERE (dbo.TxCoreV2.TxAmount <> 0) 
                            AND (dbo.TxCoreV2.TxDate BETWEEN CONVERT(DATETIME, @p0, 102) AND CONVERT(DATETIME, @p1, 102))
                        GROUP BY dbo.ExpenseGroup.Note, dbo.ExpenseGroup.Name, dbo.TxCategory.TL_Number, dbo.TxCategory.Name, 
                            dbo.TxCategory.DeductiblePercentage, dbo.ExpenseGroup.Id, dbo.TxMoneySrc.Owner
                        HAVING (dbo.TxMoneySrc.Owner = @p2) AND (dbo.TxCategory.TL_Number IS NOT NULL) AND (dbo.TxCategory.TL_Number <= 201)
                        ORDER BY dbo.ExpenseGroup.Note, ISNULL(dbo.TxCategory.TL_Number, 999)";

          var data = await context.VwTaxLiqReports
              .FromSqlRaw(sql, startDateStr, endDateStr, owner)
              .ToListAsync();

          return data;
        }
      }
      catch (Exception ex)
      {
        // Log the exception
        Console.WriteLine($"Error in GetTaxReportDataFromRawSqlAsync: {ex.Message}");
        throw; // Rethrow to be handled by caller
      }
    }
  }
}