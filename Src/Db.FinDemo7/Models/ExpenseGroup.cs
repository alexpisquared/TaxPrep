using System;
using System.Collections.Generic;

namespace Db.FinDemo7.Models;

public partial class ExpenseGroup
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string Note { get; set; }

    public virtual ICollection<TxCategory> TxCategories { get; } = new List<TxCategory>();
}
