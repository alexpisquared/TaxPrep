using AAV.WPF.Ext;
using AsLink;
using Db.FinDemo.DbModel;
using MF.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MF.TxCategoryAssigner
{
  public class ImportToDB
  {
    delegate void FileProc(string file);
    static readonly A0DbContext _db = A0DbContext.Create();
    static DateTime _now = DateTime.Now;

    public static void ImportAllAtOnce()
    {
      _now = DateTime.Now;
      LoadFolderCsvsToDb(PreSet.PathToCibcVisa, "cibccreditcard*.ofx", Vs);
      UpdateTxCoreWith_VI();

      LoadFolderCsvsToDb(PreSet.PathToPcMaster, "report*.csv", Mc);
      UpdateTxCoreWith_MC();

      LoadFolderCsvsToDb(PreSet.PathToTdAavPro, "account*.csv", Td);
      LoadFolderCsvsToDb(PreSet.PathToTdPerson, "account*.csv", Td);
      UpdateTxCoreWith_TD();

      DbSaveMsgBox.CheckAskSave(_db);
    }

    static void LoadFolderCsvsToDb(string srcDir, string wildcard, FileProc fp)
    {
      try { Directory.EnumerateFiles(srcDir, wildcard, SearchOption.TopDirectoryOnly).ToList().ForEach(file => fp(file)); }
      catch (UnauthorizedAccessException UAEx) { Debug.WriteLine(UAEx.Message); throw; }
      catch (Exception ex) { ex.Pop(); }
    }

    static void Mc(string file) => UpdateDbWithDt_MC(ExcelHlp.DataTableFromXlsOrCsv(file, true));
    static void Vs(string file) => UpdateDbWith___VI(MsMoneyHlp.DataTableFromOfx(file));

    static void Td(string file) => UpdateDbWithDt_TD(ExcelHlp.DataTableFromXlsOrCsv(file, false), file.Contains(@"\" + PreSet.__TdCo + @"\"));

    static void UpdateDbWith___VI(List<TxCore_CiVi> list)
    {
      try
      {
        foreach (var nr in list)
        {
          nr.CreatedAt = _now;

          if (!_db.TxCore_CiVi.Any(r => r.FitId == nr.FitId)) // 						Trace.WriteLine(string.Format("The row  {0}  is already in DB.", nr.FitId));
            _db.TxCore_CiVi.Add(nr);
        }
      }
      catch (Exception ex) { ex.Pop(); }
    }
    static void UpdateDbWithDt_VI(DataTable dt)
    {
      try
      {
        foreach (DataRow dr in dt.Rows)
        {
          var nr = new TxCore_CiVi
          {
            DtPosted = (DateTime)dr["Date"],
            Details = dr["Details"] is System.DBNull ? null : (string)dr["Details"],
            Credit = dr["Credit"] is System.DBNull ? null : (decimal?)(double)dr["Credit"],
            Debit = dr["Debit"] is System.DBNull ? null : (decimal?)(double)dr["Debit"],
            CreatedAt = _now
          };

          if (_db.TxCore_CiVi.Any(r => r.DtPosted == nr.DtPosted && r.Details == nr.Details && r.Credit == nr.Credit && r.Debit == nr.Debit))
            Trace.WriteLine(string.Format("The row  {0}  is already in DB.", dr.ToString()));
          else
            _db.TxCore_CiVi.Add(nr);
        }

        //var sr = DbSaveLib.TrySaveReport(_db);
        //Trace.WriteLine(string.Format("{0}.", sr));
      }
      catch (Exception ex) { ex.Pop(); }
    }
    static void UpdateDbWithDt_TD(DataTable dt, bool isCo)
    {
      try
      {
        foreach (DataColumn dc in dt.Columns) Debug.WriteLine("{0} = dr[\"{0}\"] is System.DBNull ? null : ({1})dr[\"{0}\"],", dc.ColumnName, dc.DataType.Name);
        foreach (DataRow dr in dt.Rows)
        {
          var nr = new TxCore_TdCt
          {
            TxDate = (DateTime)dr["F1"],
            TxDescrn = dr["F2"] is System.DBNull ? null : (string)dr["F2"],
            TxAmountCrt = dr["F3"] is System.DBNull ? null : (decimal?)(double)dr["F3"],
            TxAmountDbt = dr["F4"] is System.DBNull ? null : (decimal?)(double)dr["F4"],
            TxAmountBlnc = (decimal)(double)dr["F5"],
            AccountId = isCo ? "Corp" : "Pers",
            CreatedAt = _now
          };

          if (!(_db.TxCore_TdCt.Any(r => r.TxDate == nr.TxDate && r.TxDescrn == nr.TxDescrn && r.TxAmountBlnc == nr.TxAmountBlnc))) //todo: does not check in mem: i.e.: dupes from CSVs are ignored here ..to be caught on dbSave.
            _db.TxCore_TdCt.Add(nr);
        }
      }
      catch (Exception ex) { ex.Pop(); }
    }
    static void UpdateDbWithDt_MC(DataTable dt)
    {
      try
      {
        foreach (DataRow dr in dt.Rows)
        {
          var refn = (string)dr["Reference Number "];
          if (_db.TxCore_PcMc.Any(r => r.Reference_Number == refn))
            Trace.WriteLine(string.Format("Reference Number  {0}  is already in DB.", refn));
          else
          {
            try
            {
              var nr = new TxCore_PcMc
              {
                Reference_Number = (string)dr["Reference Number "],
                Transaction_Date = (DateTime)dr["Transaction Date"],
                Posting_Date = (DateTime)dr["Posting Date"],
                Billing_Amount = (decimal)dr["Billing Amount"],
                Merchant = dr["Merchant"] is System.DBNull ? null : (string)dr["Merchant"],
                Merchant_City = dr["Merchant City "] is System.DBNull ? null : (string)dr["Merchant City "],
                Merchant_State = dr["Merchant State "] is System.DBNull ? null : (string)dr["Merchant State "],
                Merchant_Zip = dr["Merchant Zip "] is System.DBNull ? null : (string)dr["Merchant Zip "],
                Debit_Credit_Flag = (string)dr["Debit/Credit Flag "],
                SICMCC_Code = dr["SICMCC Code"].ToString(),
                CreatedAt = _now
              };

              _db.TxCore_PcMc.Add(nr);
            }
            catch (Exception ex) { ex.Pop(); }
          }
        }

        //var sr = DbSaveLib.TrySaveReport(_db);
        //Trace.WriteLine(string.Format("{0}.", sr));
      }
      catch (Exception ex) { ex.Pop(); }
    }
    static void UpdateTxCoreWith_TD()
    {
      try
      {
        foreach (var dr in _db.TxCore_TdCt.Where(r => r.TxAmountCrt != null))
        {
          if (!(_db.TxCores.Any(r => r.TxAmount == dr.TxAmountCrt && r.TxDate == dr.TxDate && r.ProductService == dr.TxDescrn && r.Notes.StartsWith(dr.TxAmountBlnc.ToString()))))  //						Trace.WriteLine(string.Format("Row  {0}  is already in DB.", dr.TxDescrn));
          {
            try
            {
              _db.TxCores.Add(new TxCore
              {
                TxAmount = dr.TxAmountCrt.Value,
                TxDate = dr.TxDate,
                ProductService = dr.TxDescrn,
                Notes = dr.TxAmountBlnc.ToString(),

                TxSupplierId = 3, // Yet Unknown.
                TxMoneySrcId = string.Compare(dr.AccountId, "Corp", true) == 0 ? PreSet.MoneySrcTdCo : string.Compare(dr.AccountId, "Pers", true) == 0 ? PreSet.MoneySrcTdPi : PreSet.MoneySrcUnKn,
                TxCategoryIdTxt = PreSet.__UnKn,
                CreatedAt = _now
              });
            }
            catch (Exception ex) { ex.Pop(); }
          }
        }
      }
      catch (Exception ex) { ex.Pop(); }
    }
    static void UpdateTxCoreWith_VI()
    {
      try
      {
        _db.TxCores.Load();
        foreach (var dr in _db.TxCore_CiVi.Where(r => r.Debit != null))
        {
          if (_db.TxCores.Local.Any(r => r.Notes.StartsWith(dr.FitId)))
          {
            //var er = _db.TxCores.Local.FirstOrDefault(r => r.Notes.StartsWith(dr.FitId));						//Trace.WriteLine(string.Format("Row  {0} is already in DB.", er.ProductService));
          }
          else
          {
            try
            {
              Trace.WriteLine(string.Format("NEW  {1}   {2} - '{0}'", dr.Details, dr.DtPosted, dr.Debit));
              _db.TxCores.Local.Add(new TxCore
              {
                ProductService = dr.Details,
                TxAmount = dr.Debit.Value,
                TxDate = dr.DtPosted,
                Notes = dr.FitId,

                TxSupplierId = 3, // Yet Unknown.
                TxMoneySrcId = PreSet.MoneySrcCiVi,
                TxCategoryIdTxt = PreSet.__UnKn,
                CreatedAt = _now
              });
            }
            catch (Exception ex) { ex.Pop(); }
          }
        }
      }
      catch (Exception ex) { ex.Pop(); }
    }
    static void UpdateTxCoreWith_MC()
    {
      try
      {
        foreach (var dr in _db.TxCore_PcMc.Where(r => r.Debit_Credit_Flag.ToUpper() == "D"))
        {
          if (_db.TxCores.Any(r => r.TxMoneySrcId == PreSet.MoneySrcPcMc && r.TxAmount == dr.Billing_Amount && r.TxDate == dr.Transaction_Date && r.Notes.StartsWith(dr.Reference_Number)))
          {
            //Trace.WriteLine(string.Format("{0}) Reference Number  {1}  is already in DB.", ++i, dr.Reference_Number));
          }
          else
          {
            try
            {
              _db.TxCores.Add(new TxCore
              {
                TxAmount = dr.Billing_Amount,
                TxDate = dr.Transaction_Date,
                ProductService = dr.Merchant,
                Notes = dr.Reference_Number,
                History = dr.Merchant_City ?? "" + " " + dr.SICMCC_Code ?? "",

                TxSupplierId = 3, // Yet Unknown.
                TxMoneySrcId = PreSet.MoneySrcPcMc,
                TxCategoryIdTxt = PreSet.__UnKn,
                CreatedAt = _now
              });
            }
            catch (Exception ex) { ex.Pop(); }
          }
        }

        //var sr = DbSaveLib.TrySaveReport(_db);
        //Trace.WriteLine(string.Format("{0}.", sr));
      }
      catch (Exception ex) { ex.Pop(); }
    }
  }


  //2014-04-07 00:00:00   113.2000 - 'MSFT *MICROSOFTSTORE BILL.MS.NE<MEMO>99.99 USD @ 1.132113'
  //2014-01-02 00:00:00   578.6700 - 'CITY OF VAUGHAN #475 THORNHILL,'

  public static class MsMoneyHlp // http://www.codeproject.com/Articles/14386/Class-to-transform-ofx-Microsoft-Money-file-into-D
  {
    public static List<TxCore_CiVi> DataTableFromOfx(string file)
    {
      Debug.WriteLine(file);

      var lst = new List<TxCore_CiVi>();
      try
      {
        foreach (var ln in File.ReadAllLines(file).Where(r => r.StartsWith("<STMTTRN>")))
        {
          var amnt1 = ln.IndexOf("<TRNAMT>") + 8;
          var amnt2 = ln.IndexOf("<FITID>");
          var dtls1 = ln.IndexOf("<NAME>");
          var dtls2 = ln.IndexOf("</STMTTRN>");
          var strAmt = ln.Substring(amnt1, amnt2 - amnt1);

          var ff = new TxCore_CiVi
          {
            DtPosted = DateTime.ParseExact(ln.Substring(ln.IndexOf("<DTPOSTED>") + 10, 8), "yyyyMMdd", CultureInfo.InvariantCulture),
            FitId = ln.Substring(amnt2 + 7, dtls1 - amnt2 - 7),
            Details = ln.Substring(dtls1 + 6, dtls2 - dtls1 - 6)
          };

          if (ln.Contains("<TRNTYPE>DEBIT<"))
            ff.Debit = -decimal.Parse(strAmt);
          else
            ff.Credit = decimal.Parse(strAmt);

          lst.Add(ff);
        }
      }
      catch (Exception ex)
      {
        Debug.WriteLine("\n *** ERROR @ {0} in {1}.{2}():\n  {3}\n", DateTime.Now.ToString("MMM yyyy HH:mm"), MethodInfo.GetCurrentMethod().DeclaringType.Name, MethodInfo.GetCurrentMethod().Name, ex.Message);
        throw;
      }

      return lst;
    }
  }

  public class Ms_Money
  {
    public bool IsDebit { get; set; }

    public DateTime DtPosted { get; set; }

    public decimal TrnAmt { get; set; }

    public string FitId { get; set; }

    public string Dtls { get; set; }
  }
}
