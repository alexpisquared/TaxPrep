namespace TxnManualEntry2022.Models;

public class TxnBook // res. book
{
  readonly IReservationProvider _reservationProvider;
  readonly IReservationCreator _reservationCreator;
  readonly IReservationConflictValidator _reservationConflictValidator;

  public TxnBook(IReservationProvider reservationProvider, IReservationCreator reservationCreator, IReservationConflictValidator reservationConflictValidator) =>
    (_reservationProvider, _reservationCreator, _reservationConflictValidator) =
       (reservationProvider, reservationCreator, reservationConflictValidator);

  public async Task<IEnumerable<AccountTxn>> GetAllTxnx() => await _reservationProvider.GetAllAccountTxnsAsync(); // _accountTxns.ToList();

  public async Task AddTxn(AccountTxn accountTxn)
  {
    AccountTxn conflictingAccountTxn = await _reservationConflictValidator.GetConflictingAccountTxn(accountTxn);
    if (conflictingAccountTxn != null)
    {
      throw new TxnConflictException(conflictingAccountTxn, accountTxn);
    }

    try
    {
      _reservationCreator.CreateReservation(accountTxn); // _accountTxns.Add(accountTxn);
    }
    catch (Exception ex)
    {
      MessageBox.Show(ex.Message);
    }
  }
}
