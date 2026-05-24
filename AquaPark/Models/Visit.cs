using System;
using System.Collections.Generic;

namespace AquaPark.Models;

public partial class Visit
{
    public int VisitId { get; set; }

    public int TicketId { get; set; }

    public DateTime EntryTime { get; set; }

    public DateTime? ExitTime { get; set; }

    public virtual Ticket Ticket { get; set; } = null!;
}
