using System;
using System.Collections.Generic;

namespace AquaPark.Models;

public partial class Zone
{
    public int ZoneId { get; set; }

    public string ZoneName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Attraction> Attractions { get; set; } = new List<Attraction>();
}
