using System;
using System.Collections.Generic;

namespace CodeverseWPF;

public partial class ViewEmployee
{
    public int EmployeeId { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string? Login { get; set; }

    public string Password { get; set; } = null!;

    public string? Email { get; set; }

    public int PositionId { get; set; }

    public string? Position { get; set; }
}
