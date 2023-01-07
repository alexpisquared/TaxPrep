using Microsoft.EntityFrameworkCore;

namespace Db.FinDemo7.Models;

public partial class FinDemoContext : DbContext
{
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    if (optionsBuilder.IsConfigured)
      return;

    _ = optionsBuilder.UseSqlServer(_sqlConnectionString, sqlServerOptions => { _ = sqlServerOptions.CommandTimeout(150).EnableRetryOnFailure(10, TimeSpan.FromSeconds(44), null); });
    _ = optionsBuilder.EnableSensitiveDataLogging();  //todo: remove for production.
  }

  [DbFunction(Name = "SOUNDEX", IsBuiltIn = true)] public static string SoundsLike(string sounds) => throw new NotImplementedException(); //tu: SOUNDEX

#if DEBUG
  const string _sqlConnectionString = """Server=.\SqlExpress;Database=FinDemoDbg;Trusted_Connection=True;Encrypt=False;""";
#else  
  const string _sqlConnectionString = """Server=.\SqlExpress;Database=FinDemo;Trusted_Connection=True;Encrypt=False;""";
#endif
}