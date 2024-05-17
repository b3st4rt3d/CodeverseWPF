using System;
using System.Collections.Generic;

namespace CodeverseWPF;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string? Login { get; set; }

    public string? Password { get; set; }

    public string? Email { get; set; }

    public int? PositionId { get; set; }

    public virtual ICollection<OrderEmployee> OrderEmployees { get; set; } = new List<OrderEmployee>();

    public virtual Position? Position { get; set; }
}
