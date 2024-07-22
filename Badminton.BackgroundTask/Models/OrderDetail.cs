using System;
using System.Collections.Generic;

namespace Badminton.BackgroundTask.Models;

public partial class OrderDetail
{
    public int OrderDetailId { get; set; }

    public int OrderId { get; set; }

    public int CourtDetailId { get; set; }

    public double Amount { get; set; }

    public virtual CourtDetail CourtDetail { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
