using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxnManualEntry2022.DTOs
{
  public class AccountTxnDTO
  {
    [Key]
    public Guid Id { get; set; }

    public int FloorNum { get; set; }
    public string RoomNum { get; set; } = default!;
    public DateTime TxnTime { get; set; }
    public decimal TxnAmount { get; set; }
    public string UserID { get; set; }
  }
}

/* ReadMe
  
add-migration Initial
update-database

*/