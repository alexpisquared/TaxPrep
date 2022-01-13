namespace TxnManualEntry2022.DbContexts;

public class TmeDbContextFactory //: IDesignTimeDbContextFactory<TmeDbContext>
{
  readonly string _connectionString;

  public TmeDbContextFactory(string connectionString) => _connectionString = connectionString;

  public TmeDbContext CreateDbContext()
  {
    var options = new DbContextOptionsBuilder().UseSqlite(_connectionString).Options;

    return new TmeDbContext(options);
  }
}
