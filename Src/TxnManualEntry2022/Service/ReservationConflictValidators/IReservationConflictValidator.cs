namespace TxnManualEntry2022.Service.ReservationConflictValidators;

public interface IReservationConflictValidator
{
  Task<AccountTxn> GetConflictingAccountTxn(AccountTxn accountTxn);
}
