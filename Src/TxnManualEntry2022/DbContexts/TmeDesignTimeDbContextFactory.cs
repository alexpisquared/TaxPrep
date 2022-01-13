using Microsoft.EntityFrameworkCore.Design;

namespace TxnManualEntry2022.DbContexts;

public class TmeDesignTimeDbContextFactory : IDesignTimeDbContextFactory<TmeDbContext>
{
  public TmeDbContext CreateDbContext(string[] args)
  {
    DbContextOptions options = new DbContextOptionsBuilder().UseSqlite("Data Source=tme.db").Options;

    return new TmeDbContext(options);
  }
}
