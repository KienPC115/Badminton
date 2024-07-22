﻿using System;
using System.Collections.Generic;

namespace Badminton.BackgroundTask.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string Phone { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime DateOfBirth { get; set; }

    public string Password { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
