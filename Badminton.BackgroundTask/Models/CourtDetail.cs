using System;
using System.Collections.Generic;

namespace Badminton.BackgroundTask.Models;

public partial class CourtDetail
{
    public int CourtDetailId { get; set; }

    public int CourtId { get; set; }

    public string Slot { get; set; } = null!;

    public double Price { get; set; }

    public string Status { get; set; } = null!;

    public string Notes { get; set; } = null!;

    public int? Capacity { get; set; }

    public int? BookingCount { get; set; }

    public virtual Court Court { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
