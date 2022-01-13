using TxnManualEntry2022.DbContexts;

namespace TxnManualEntry2022.Service;

public class DatabaseReservationProvider : IReservationProvider
{
  readonly TmeDbContextFactory _dbContextFactory;

  public DatabaseReservationProvider(TmeDbContextFactory dbContextFactory) => _dbContextFactory = dbContextFactory;

  public async Task<IEnumerable<AccountTxn>> GetAllAccountTxnsAsync()
  {
    using TmeDbContext context = _dbContextFactory.CreateDbContext();

    IEnumerable<AccountTxnDTO> accountTxnDTOs = await context.AccountTxns.ToListAsync();

    await Task.Delay(1500);

    return accountTxnDTOs.Select(r => ToAccountTxn(r));
  }

  private static AccountTxn ToAccountTxn(AccountTxnDTO r) => new AccountTxn(new TxnTypeID(r.FloorNum, r.RoomNum), r.TxnTime, r.UserID);
}
