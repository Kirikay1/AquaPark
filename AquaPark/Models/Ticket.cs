using System;
using System.Collections.Generic;

namespace AquaPark.Models;

public partial class Ticket
{
    public int TicketId { get; set; }

    public int TicketTypeId { get; set; }

    public int? ClientId { get; set; }

    public DateTime PurchaseDate { get; set; }

    public DateOnly VisitDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual Client? Client { get; set; }

    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();

    public virtual TicketType TicketType { get; set; } = null!;

    public virtual ICollection<Visit> Visits { get; set; } = new List<Visit>();
}
