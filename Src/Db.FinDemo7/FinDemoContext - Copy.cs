using Microsoft.EntityFrameworkCore;

namespace Db.FinDemo7.Models;

public partial class FinDemoContext : DbContext
{
  readonly string _sqlConnectionString = "<Not Initialized!!!>";//todo: if not done: remove warnig and ... in protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)  #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
  
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    if (optionsBuilder.IsConfigured)
      return;

    _ = optionsBuilder.UseSqlServer(_sqlConnectionString, sqlServerOptions => { _ = sqlServerOptions.CommandTimeout(150).EnableRetryOnFailure(10, TimeSpan.FromSeconds(44), null); });
    _ = optionsBuilder.EnableSensitiveDataLogging();  //todo: remove for production.
  }

  public FinDemoContext(string connectoinString)
  {
    _sqlConnectionString = connectoinString;
#if DEBUG_
      if (Debugger.IsAttached && System.Environment.UserDomainName == "RAZER1")
      {
        Debugger.Break();
        Database.EnsureCreated();
      }
#endif
  }


  [DbFunction(Name = "SOUNDEX", IsBuiltIn = true)] public static string SoundsLike(string sounds) => throw new NotImplementedException(); //tu: SOUNDEX

//#if DEBUG
//  const string _sqlConnectionString = """Server=.\SqlExpress;Database=FinDemoDbg;Trusted_Connection=True;Encrypt=False;""";
//#else  
//  const string _sqlConnectionString = """Server=.\SqlExpress;Database=FinDemo;Trusted_Connection=True;Encrypt=False;""";
//#endif
}