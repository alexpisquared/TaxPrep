namespace TxnManualEntry2022.Service;

public interface IReservationProvider
{
  Task<IEnumerable<AccountTxn>> GetAllAccountsAsync();
}
