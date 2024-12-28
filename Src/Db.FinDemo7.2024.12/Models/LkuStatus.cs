using System;
using System.Collections.Generic;

namespace Db.FinDemo7.Models;

public partial class LkuStatus
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }
}
