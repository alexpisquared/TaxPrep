namespace TxnManualEntry2022.Service.ReservationConflictValidators;

public class DatabaseReservationConflictValidator : IReservationConflictValidator
{
  readonly TmeDbContextFactory _dbContextFactory;
  public DatabaseReservationConflictValidator(TmeDbContextFactory dbContextFactory) => _dbContextFactory = dbContextFactory;

  public async Task<AccountTxn> GetConflictingAccountTxn(AccountTxn accountTxn)
  {
    using var context = _dbContextFactory.CreateDbContext();

    var dto = await context.AccountTxns
      .Where(r => r.FloorNum == accountTxn.TxnTypeID.FloorNum)
      .Where(r => r.RoomNum == accountTxn.TxnTypeID.RoomNum)
      .Where(r => r.TxnAmount == accountTxn.TxnAmount)
      .Where(r => r.TxnTime == accountTxn.TxnTime)
      .FirstOrDefaultAsync();

    if(dto == null) return null;

    return ToAccountTxn(dto);
  }

  private static AccountTxn ToAccountTxn(AccountTxnDTO r) => new AccountTxn(new TxnTypeID(r.FloorNum, r.RoomNum), r.TxnTime, r.UserID);
}
