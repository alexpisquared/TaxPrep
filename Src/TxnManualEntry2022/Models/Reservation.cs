﻿namespace TxnManualEntry2022.Models;

public class AccountTxn // res-n
{
  public AccountTxn(TxnTypeID txnTypeID, DateTime txnTime, string userID)
  {
    TxnTypeID = txnTypeID;
    TxnTime = txnTime;
    UserID = userID;
  }

  public TxnTypeID TxnTypeID { get; }
  public DateTime TxnTime { get; }
  public decimal TxnAmount { get; }
  public string UserID { get; }

  public bool Conflicts(AccountTxn accountTxn) => accountTxn.TxnTime - TxnTime < new TimeSpan(24, 0, 0) &&  // if on the same date
      accountTxn.TxnAmount - TxnAmount < .01m;                  // if of the same value
}
