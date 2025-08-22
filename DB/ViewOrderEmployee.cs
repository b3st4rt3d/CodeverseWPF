using System;
using System.Collections.Generic;

namespace CodeverseWPF.DB;

public partial class ViewOrderEmployee
{
    public int EmployeeId { get; set; }

    public int OrderId { get; set; }

    public int? ClientId { get; set; }

    public decimal? Total { get; set; }

    public int StateId { get; set; }

    public string State { get; set; } = null!;

    public string? Background { get; set; }

    public int? PositionId { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public DateOnly? Date { get; set; }
}
