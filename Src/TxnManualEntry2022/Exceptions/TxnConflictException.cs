using TxnManualEntry2022.Models;

namespace TxnManualEntry2022.Exceptions;

[Serializable]
internal class TxnConflictException : Exception
{
  public AccountTxn ExistingAccountTxn { get; }
  public AccountTxn IncomingAccountTxn { get; }
  
  public TxnConflictException()
  {
  }

  public TxnConflictException(string? message) : base(message)
  {
  }

  public TxnConflictException(string? message, Exception? innerException) : base(message, innerException)
  {
  }

  protected TxnConflictException(SerializationInfo info, StreamingContext context) : base(info, context)
  {
  }

  public TxnConflictException(AccountTxn existingAccountTxn, AccountTxn incomingAccountTxn)
  {
    ExistingAccountTxn = existingAccountTxn;
    IncomingAccountTxn = incomingAccountTxn;
  }
}
