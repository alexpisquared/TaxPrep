using System;
using System.Collections.Generic;

namespace Db.FinDemo7.Models;

public partial class VwDupesDetail
{
    public decimal TxAmount { get; set; }

    public DateTime TxDate { get; set; }

    public string TxCategoryIdTxt { get; set; }

    public string ProductService { get; set; }

    public string Notes { get; set; }

    public string Supplier { get; set; }

    public string MoneySrc { get; set; }

    public string History { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }
}
