using System;
using System.Collections.Generic;

namespace AquaPark.Models;

public partial class Attraction
{
    public int AttractionId { get; set; }

    public string AttractionName { get; set; } = null!;

    public int ZoneId { get; set; }

    public string? Description { get; set; }

    public int AgeLimit { get; set; }

    public int? HeightLimit { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<AttractionSchedule> AttractionSchedules { get; set; } = new List<AttractionSchedule>();

    public virtual Zone Zone { get; set; } = null!;
}
