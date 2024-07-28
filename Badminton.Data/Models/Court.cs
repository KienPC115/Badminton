using System;
using System.Collections.Generic;

namespace Badminton.Data.Models;

public partial class Court
{
    public int CourtId { get; set; }

    public string Name { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? Description { get; set; }

    public string YardType { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string Location { get; set; } = null!;

    public string SpaceType { get; set; } = null!;

    public DateTime? CreatedTime { get; set; }

    public DateTime? UpdatedTime { get; set; }

    public double Price { get; set; }

    public virtual ICollection<CourtDetail> CourtDetails { get; set; } = new List<CourtDetail>();
}
