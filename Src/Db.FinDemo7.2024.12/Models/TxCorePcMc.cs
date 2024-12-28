using System;
using System.Collections.Generic;

namespace Db.FinDemo7.Models;

public partial class TxCorePcMc
{
    public int Id { get; set; }

    public DateTime TransactionDate { get; set; }

    public DateTime PostingDate { get; set; }

    public decimal BillingAmount { get; set; }

    public string Merchant { get; set; } = null!;

    public string? MerchantCity { get; set; }

    public string? MerchantState { get; set; }

    public string? MerchantZip { get; set; }

    public string ReferenceNumber { get; set; } = null!;

    public string DebitCreditFlag { get; set; } = null!;

    public string SicmccCode { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public string? Notes { get; set; }
}
