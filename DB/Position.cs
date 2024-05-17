using System;
using System.Collections.Generic;

namespace CodeverseWPF;

public partial class Position
{
    public int PositionId { get; set; }

    public string? Position1 { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
