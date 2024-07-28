using System;
using System.Collections.Generic;

namespace Badminton.Data.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int CustomerId { get; set; }

    public string Type { get; set; } = null!;

    public DateTime OrderDate { get; set; }

    public string? OrderNotes { get; set; }

    public double TotalAmount { get; set; }

    public string? OrderStatus { get; set; }

    public string? PhoneOrder { get; set; }

    public string? Email { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
