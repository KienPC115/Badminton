using System;
using System.Collections.Generic;

namespace Badminton.Data.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string Phone { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime DateOfBirth { get; set; }

    public virtual ICollection<CourtDetail> CourtDetails { get; set; } = new List<CourtDetail>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
