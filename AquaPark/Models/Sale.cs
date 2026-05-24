using System;
using System.Collections.Generic;

namespace AquaPark.Models;

public partial class Sale
{
    public int SaleId { get; set; }

    public int TicketId { get; set; }

    public int EmployeeId { get; set; }

    public DateTime SaleDate { get; set; }

    public decimal TotalAmount { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Ticket Ticket { get; set; } = null!;
}
