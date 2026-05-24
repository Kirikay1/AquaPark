using System;
using System.Collections.Generic;

namespace AquaPark.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int SaleId { get; set; }

    public DateTime PaymentDate { get; set; }

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string PaymentStatus { get; set; } = null!;

    public virtual Sale Sale { get; set; } = null!;
}
