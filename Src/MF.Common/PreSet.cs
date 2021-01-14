using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.Common
{
  public static class PreSet
  {
    const string
      _ofc = @"C:\c\Live\MinFin\MSMoneyDbLoader\Assets\",
      _hom = @"C:\Users\alexp\OneDrive\Documents\0\MF\DnLds\";

    public const int MoneySrcPcMc = 5, MoneySrcCiVi = 8, MoneySrcTdCo = 3, MoneySrcTdPi = 2, MoneySrcUnKn = 0;

    public static string TrgFormat = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"0\MF\DnLds\{0}\{1}{2}");
    public const string
          AcntIdTAg = "<ACCTID>",
          __Cash = "Cash",
          __TdPi = "TdPi",
          __TdCo = "TdCo",
          __NUse = "NUse",
          __PcMc = "PcMc",
          __UnKn = "UnKn",
          __JMVi = "JMVi",
          __CiVi = "CiVi",
          __AmEx = "AmEx",
          __PcDt = "PcDt",
          __JMMc = "JMMc",
          __Amzn = "amzn",
          DbSynced = "DB-Synced on",
          BaSynced = "Use later DbSynced for both";

    public static string PathToDnLdRoot => Environment.UserDomainName == "OFFICE" ? _ofc : _hom;
    public static string PathToCibcVisa => Environment.UserDomainName == "OFFICE" ? _ofc : Path.Combine(_hom, __CiVi);  // string.Format(@"{0}{1}\", hom, __CiVi); } }
    public static string PathToPcMaster => Environment.UserDomainName == "OFFICE" ? _ofc : Path.Combine(_hom, __PcMc);
    public static string PathToTdPerson => Environment.UserDomainName == "OFFICE" ? _ofc : Path.Combine(_hom, __TdPi);
    public static string PathToTdAavPro => Environment.UserDomainName == "OFFICE" ? _ofc : Path.Combine(_hom, __TdCo);
    public static string Downloads => Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", "{374DE290-123F-4565-9164-39C4925E467B}", String.Empty).ToString(); //tu:


    public static string GetAcntFolder(string acntId)
    {
      int len = acntId.Length;
      string acnt4 = acntId.Substring(len - 4, 4);
      switch (acnt4)
      {
        case "9889": return __TdPi;
        case "9741": return __TdCo;
        case "4630": // have no idea why this is that. 
        case "9463": return __PcMc;
        case "2689": return __JMVi;
        case "2080": return __JMMc; // starting from 2016-11
        case "3729":
        case "8546": return __CiVi;
        default: break;
      }
      return acnt4;
    }
  }
}
