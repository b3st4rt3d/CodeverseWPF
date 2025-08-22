using System;
using System.Collections.Generic;

namespace CodeverseWPF.DB;

public partial class ViewEmployee
{
    public int EmployeeId { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string? Login { get; set; }

    public string? Password { get; set; }

    public string? Email { get; set; }

    public int PositionId { get; set; }

    public string? Position { get; set; }

    public byte[]? Image { get; set; }
}
