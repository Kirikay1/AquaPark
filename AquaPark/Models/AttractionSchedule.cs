using System;
using System.Collections.Generic;

namespace AquaPark.Models;

public partial class AttractionSchedule
{
    public int ScheduleId { get; set; }

    public int AttractionId { get; set; }

    public DateOnly WorkDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string Status { get; set; } = null!;

    public virtual Attraction Attraction { get; set; } = null!;
}
