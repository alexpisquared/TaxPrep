#nullable disable
using Microsoft.EntityFrameworkCore;
namespace Db.FinDemo.PowerTools.Models;

public partial class FinDemoContext
{
  readonly string _sqlConnectionString = "<Not Initialized!!!>";//todo: if not done: remove warning and ... in protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)  #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.

  public FinDemoContext(string connectionString)
  {
    _sqlConnectionString = IsDbg ? @"Server=.\SqlExpress;Database=FinDemo;Trusted_Connection=True;TrustServerCertificate=Yes;Encrypt=False;Connection Timeout=15;" : connectionString;
#if EnsureCreated_
      if (Debugger.IsAttached && System.Environment.UserDomainName == "RAZER1")
      {
        Debugger.Break();
        Database.EnsureCreated();
      }
#endif
  }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) //tu: 
  {
    if (!optionsBuilder.IsConfigured)
    {
      _ = optionsBuilder.UseSqlServer(_sqlConnectionString, sqlServerOptions => { _ = sqlServerOptions.CommandTimeout(150).EnableRetryOnFailure(10, TimeSpan.FromSeconds(44), null); });
      _ = optionsBuilder.EnableSensitiveDataLogging();  //todo: remove for production.                                                                                                                                                                                                                                 
    }
  }

  public static bool IsDbg =>
#if DEBUG
      true;
#else
      false;
#endif

  [DbFunction(Name = "SOUNDEX", IsBuiltIn = true)] public static string SoundsLike(string sounds) => throw new NotImplementedException(); //tu: SOUNDEX
}