using System;
using System.Collections.Generic;

namespace CodeverseWPF;

public partial class OrderEmployee
{
    public int OrderEnployeeId { get; set; }

    public int OrderId { get; set; }

    public int EmployeeId { get; set; }

    public DateTime? Date { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
