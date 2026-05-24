using System;
using System.Collections.Generic;

namespace AquaPark.Models;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public int UserId { get; set; }

    public string Position { get; set; } = null!;

    public DateOnly HireDate { get; set; }

    public decimal? Salary { get; set; }

    public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();

    public virtual User User { get; set; } = null!;
}
