using AAV.Sys.Ext;
using AsLink;
using Db.FinDemo.DbModel;
using MF.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace MSMoneyFormatAdapter
{
  public class MSMoneyFileReader
  {
    public static DateTime _batchTimeNow = DateTime.Now;
    static int _cntr = 0;
    private const string _separtor = "\",\"";
    private const string _header = "\"Merchant Name\",\"Card Used For Transaction\",\"Date\",\"Time\",\"Amount\"";

    public static List<TxCoreV2> ReadTxs(A0DbContext db, string file, out string fla_acntDir, out int txMoneySrcId)
    {
      Debug.WriteLine(Path.GetFileName(file), "\n\n");

      var txns = new List<TxCoreV2>();
      txMoneySrcId = -1;

      try
      {
        if (string.Equals(Path.GetExtension(file), ".csv", StringComparison.OrdinalIgnoreCase)) // PC since 2019
        {
          var lines = File.ReadAllLines(file);
          if (lines.Length < 2) { fla_acntDir = $"Is empty file: {file}"; return txns; }
          if (!lines[0].Equals(_header)) { fla_acntDir = $"Is not a known fin file: {file}"; return txns; }

          var acntId = lines[1].Split(new[] { _separtor }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("*", "");
          txMoneySrcId = getCreateTxMoneySrc(db, out fla_acntDir, acntId, Path.GetFileName(file));

          foreach (var line in lines)
          {
            if (line.Equals(_header)) continue;
            if (string.IsNullOrEmpty(line)) continue;
            var cells = line.Split(new[] { _separtor }, StringSplitOptions.RemoveEmptyEntries);
            if (cells.Length < 3) continue;

            txns.Add(new TxCoreV2
            {
              Id = --_cntr,
              CreatedAt = _batchTimeNow,
              FitId = line,
              TxDate = parseTxnDate(cells[2], cells[3]),
              TxAmount = parseTA(cells[4].Replace("\"", "")),
              TxDetail = cells[0].Substring(1),
              //MemoPP = mm + cs + td + si,
              TxCategoryIdTxt = PreSet.__UnKn,
              TxMoneySrcId = txMoneySrcId,
              SrcFile = Path.GetFileNameWithoutExtension(file)
            });
          }
        }
        else if (string.Equals(Path.GetExtension(file), ".qif", StringComparison.OrdinalIgnoreCase)) // amazon / chase qif file format
        {
          var blocks = File.ReadAllText(file).Split(new string[] { "^" }, StringSplitOptions.RemoveEmptyEntries);
          var acntId = "amzn"; //only for amazon since no other formats available; note: no CC # in qif ==> use for amazon only.
          txMoneySrcId = getCreateTxMoneySrc(db, out fla_acntDir, acntId, Path.GetFileName(file));

          foreach (var block in blocks)
            if (block.Contains("\nD"))
              txns.Add(doTxnBlockQif(block, file, txMoneySrcId));
        }
        else // .ofx .qfx .aso
        {
          var blocks = File.ReadAllText(file).Split(new string[] { "</STMTTRN>" }, StringSplitOptions.RemoveEmptyEntries);

          var acntId = blocks[0].Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(r => r.Contains(PreSet.AcntIdTAg));
          if (acntId?.Length > 20) // Jun2017: elements not always are one per line.
            acntId = getElCont(acntId, PreSet.AcntIdTAg);

          txMoneySrcId = getCreateTxMoneySrc(db, out fla_acntDir, acntId, Path.GetFileName(file));

          foreach (var block in blocks)
            if (block.Contains("<STMTTRN>"))
              txns.Add(doTxnBlock(block.Replace("\n", "").Replace("\r", ""), file, txMoneySrcId));
        }
      }
      catch (Exception ex) { ex.Log(); fla_acntDir = "Exceptioned"; }

      return txns;
    }

    static int getCreateTxMoneySrc(A0DbContext db, out string acntDir, string acntId, string filename)
    {
      var ad = acntDir = PreSet.GetAcntFolder((acntId ?? "AcntUnknown").Replace(PreSet.AcntIdTAg, "").Trim());
      var txMoneySrcs = db.TxMoneySrcs.FirstOrDefault(r => string.Compare(ad, r.Fla, true) == 0);
      if (txMoneySrcs == null)
      {
        txMoneySrcs = db.TxMoneySrcs.Add(new TxMoneySrc { CreatedAt = _batchTimeNow, Fla = acntDir, Name = acntDir, Notes = $"Auto created from '{filename}'" });
        db.SaveChanges();
        new System.Speech.Synthesis.SpeechSynthesizer().Speak($"New account {acntDir} created from the file {filename}.");
      }

      if (txMoneySrcs == null)
      {
        //todo:        new System.Speech.Synthesis.SpeechSynthesizer().SpeakAsync($"Unable to establich source account. Exiting");
        if (Debugger.IsAttached)
          Debugger.Break();
        else
          throw new NullReferenceException($"Unable to establich source account. Exiting");
      }

      return txMoneySrcs.Id;
    }

    public const string LedgerBal = "LEDGERBAL", AvailbBal = "AVAILBAL";
    public static List<BalAmtHist> ReadBAH(string file, int txMoneySrcId, string balTpe = LedgerBal) // for all files, or AVAILBAL for VIsa, TD only.
    {
      var rv = new List<BalAmtHist>();

      var tagBgn = string.Format("<{0}>", balTpe);
      var tagEnd = string.Format("</{0}>", balTpe);
      var blocks = File.ReadAllText(file).Split(new string[] { tagEnd }, StringSplitOptions.RemoveEmptyEntries);
      foreach (var block in blocks)
        if (block.Contains(tagBgn))
          rv.Add(doBalAmtBlock(block.Replace("\n", "").Replace("\r", ""), txMoneySrcId, tagBgn, balTpe));

      return rv;
    }

    static int inferTxMoneySrcId(string file)
    {
      var dirs = file.Split('\\');
      var dir = dirs[dirs.Length - 2];
      switch (dir)
      {
        default: return 6;      //0	 is not working with db ini.
        case PreSet.__Cash: return 1; //1	 Cash	
        case PreSet.__TdPi: return 2; //2	 Debit*1301 (Alex)	
        case PreSet.__TdCo: return 3; //3	 biz Debit*3368 (AAVpro)	Debit Card
        case PreSet.__NUse: return 4; //4	 zVISA (old CIBC)	NOT USED
        case PreSet.__PcMc: return 5; //5	 MC*9463 (Alex)	
        case PreSet.__UnKn: return 6; //6	 New Way!	not sure ..cheques...
        case PreSet.__JMVi: return 7; //7	 wVISA*2689 (Mei)	
        case PreSet.__CiVi: return 8; //8	 VISA*3729 (Alex)	
        case PreSet.__AmEx: return 9; //9	 AmEx (Mei)	
        case PreSet.__PcDt: return 10;  //a	 PC Debit*???? (Mei)	Meis' chequing accnt}

        case PreSet.__Amzn: return 13;
        case PreSet.__JMMc: return 13;
      }
    }
    static BalAmtHist doBalAmtBlock(string block, int txMoneySrcId, string tagBgn, string balTpe)
    {
      var start = block.IndexOf(tagBgn);
      if (start < 0) return null;

      var dp = getElCont(block, "<DTASOF>"); // as of date
      var ta = getElCont(block, "<BALAMT>");

      var rv = new BalAmtHist
      {
        Id = --_cntr,
        CreatedAt = _batchTimeNow,
        AsOfDate = parseTxnDate(dp),
        BalAmt = -parseTA(ta),
        BalTpe = balTpe,
        TxMoneySrcId = txMoneySrcId // inferTxMoneySrcId(file), // fix this Jan 30 2019
      };

      return rv;
    }
    static TxCoreV2 doTxnBlockQif(string block, string file, int txMoneySrcId)
    {
      try
      {
        var ls = block.Split(new[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        var rv = new TxCoreV2
        {
          Id = --_cntr,
          CreatedAt       /**/ = _batchTimeNow,
          FitId           /**/ = ls.FirstOrDefault(r => r[0] == 'N').Substring(1),
          TxDate          /**/ = parseTxnDate(ls.FirstOrDefault(r => r[0] == 'D').Substring(1)),
          TxAmount        /**/ = -parseTA(ls.FirstOrDefault(r => r[0] == 'T').Substring(1)),
          TxDetail        /**/ = ls.FirstOrDefault(r => r[0] == 'P').Substring(1),
          MemoPP          /**/ = string.Join(", ", ls.Where(r => r[0] == 'A' && r.Length > 1).Select(r => r.Substring(1))) + ". ",
          TxCategoryIdTxt /**/ = PreSet.__UnKn,
          TxMoneySrcId    /**/ = txMoneySrcId, // inferTxMoneySrcId(file),
          SrcFile         /**/ = Path.GetFileNameWithoutExtension(file)
        };

        Debug.WriteLine(rv);

        return rv;
      }
      catch (Exception ex) { ex.Log(); }

      return null;
    }
    static TxCoreV2 doTxnBlock(string block, string file, int txMoneySrcId)
    {
      var start = block.IndexOf("<STMTTRN>");
      if (start < 0) return null;

      //Debug.WriteLine(block.Substring(33).Replace("<TRNAMT>", "\t").Replace("<FITID>", "\t").Replace("<NAME>", "\t\t").Replace("<MEMO>", "\t\t").Replace("<CHECKNUM>", "\t\t").Replace("<DTUSER>", "\t\t").Replace("<SIC>", "\t\t"));

      //var tt = getElCont(block, "<TRNTYPE>");
      var dp = getElCont(block, "<DTPOSTED>");     // posted date
      var ta = getElCont(block, "<TRNAMT>");
      var fi = getElCont(block, "<FITID>");
      var dt = getElCont(block, "<NAME>") ?? "°";  // required db field
      var mm = getElCont(block, "<MEMO>");         // CV only: details of $US txn.
      var cs = getElCont(block, "<CHECKNUM>");     // TD only: cheque number
      var td = getElCont(block, "<DTUSER>");       // MC only: tx date
      var si = getElCont(block, "<SIC>");          // MC only: 4-digits like 0000, 4816, ...

      if (td != null) td = string.Format("txDate:{0}, ", parseTxnDate(td));
      if (si != null) si = string.Format("sic:{0}", si);

      var rv = new TxCoreV2
      {
        Id = --_cntr,
        CreatedAt = _batchTimeNow,
        FitId = fi,
        TxDate = parseTxnDate(dp),
        TxAmount = -parseTA(ta),
        TxDetail = dt,
        MemoPP = mm + cs + td + si,
        TxCategoryIdTxt = PreSet.__UnKn,
        TxMoneySrcId = txMoneySrcId, // inferTxMoneySrcId(file),
        SrcFile = Path.GetFileNameWithoutExtension(file)
      };

      //Debug.WriteLine(rv);

      return rv;
    }
    static DateTime parseTxnDate(string mdy, string hmm_m)
    {
      if (!DateTime.TryParseExact($"{mdy} {hmm_m}", "MM/dd/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out var ta))
        throw new Exception($"Bad date:    {mdy} {hmm_m}");

      if (ta > _batchTimeNow)
      {
        Trace.WriteLine($"PC MC bug: date corrected from future {ta} to past {ta.AddYears(-1)}");
        ta = ta.AddYears(-1);
      }

      return ta;
    }
    static DateTime parseTxnDate(string dp)
    {
      var dv = dp.Trim().Split('[')[0];
      var f = "yyyyMMddHHmmss.fff".Substring(0, dv.Length);
      if (DateTime.TryParseExact(dv, f, CultureInfo.InvariantCulture, DateTimeStyles.None, out var ta))
        return ta;

      f = "MM/dd/yyyy";
      if (DateTime.TryParseExact(dv, f, CultureInfo.InvariantCulture, DateTimeStyles.None, out ta))
        return ta;


      {
        Debug.WriteLine("\n{0}\n{1}", f, dp);
        throw new Exception("Bad amount: " + dp);
      }
    }
    static decimal parseTA(string ts)
    {
      if (!decimal.TryParse(ts.Replace("$", ""), out var ta)) throw new Exception("Bad amount: " + ts);
      return ta;
    }
    static string getElCont(string block, string p)
    {
      var start = block.IndexOf(p);
      if (start < 0) return null;

      var rv = block.Substring(start + p.Length);
      var r2 = rv.Split('<')[0];      //Debug.WriteLine("    '{0}' {1}", r2, "    ");
      return r2.Trim();
    }
  }

}
/*
 Using the new code (LinqToXml)

 XElement doc = ImportOfx.ToXElement(pathToOfx);
//queryiny the XElement
var imps = (from c in doc.Descendants("STMTTRN")
            where c.Element("TRNTYPE").Value == "DEBIT"
            select new tb_import
            {
                amount = decimal.Parse(c.Element("TRNAMT").Value.Replace("-", ""),
                                       NumberFormatInfo.InvariantInfo),
                data = DateTime.ParseExact(c.Element("DTPOSTED").Value, 
                                           "yyyyMMdd", null),
                description = c.Element("MEMO").Value,
                id_account = id_account
            });

 * These are the columns inside the DataSet:

TRNTYPE - Type of transaction: DEBIT or CREDIT
DTPOSTED - Date of the transaction, formatted YYYYMMDDHHMMSS
TRNAMT - Amount (negative when it is a DEBIT)
FITID - Transaction ID CHECKNUM - Number of the check or transaction ID
MEMO - Small description of the transaction; very useful when you use your debit card

 * 
 
 The plan
 * new db schema:
 *	- one TxCore for all possible types:
 *		contains FitId string column
 *		see if we miss anything from CSV (txn vs post date: have to use post since not in MsMoney.)
 * import all into the new paralell schema and see if totals match
 *		
 * 
 */
