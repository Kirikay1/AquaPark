using System;
using System.Collections.Generic;

namespace AquaPark.Models;

public partial class TicketType
{
    public int TicketTypeId { get; set; }

    public string TicketName { get; set; } = null!;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int DurationHours { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
