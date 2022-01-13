namespace TxnManualEntry2022.DbContexts;

public class TmeDbContext : DbContext
{
  public TmeDbContext(DbContextOptions options) : base(options)
  {
  }

  public DbSet<AccountTxnDTO> AccountTxns { get; set; }
}
