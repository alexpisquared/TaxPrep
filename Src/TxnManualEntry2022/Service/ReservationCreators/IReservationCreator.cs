using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxnManualEntry2022.Service.ReservationCreators;

public interface IReservationCreator
{
  Task CreateReservation(AccountTxn txn);
}

public class DatabaseReservationCreator : IReservationCreator
{
  readonly TmeDbContextFactory _dbContextFactory;
  public DatabaseReservationCreator(TmeDbContextFactory dbContextFactory) => _dbContextFactory = dbContextFactory;

  public async Task CreateReservation(AccountTxn txn)
  {
    using TmeDbContext context = _dbContextFactory.CreateDbContext();
    AccountTxnDTO accountTxnDTO = ToTmeDTO(txn);

    context.AccountTxns.Add(accountTxnDTO);
    await context.SaveChangesAsync();
  }

  private AccountTxnDTO ToTmeDTO(AccountTxn txn) => new AccountTxnDTO()
  {
    FloorNum = txn.TxnTypeID?.FloorNum ?? -1,
    RoomNum = txn.TxnTypeID?.RoomNum ?? "-a",
    TxnAmount = txn.TxnAmount,
    TxnTime = txn.TxnTime,
    UserID = txn.UserID

  };
}
