using System;
using System.Collections.Generic;

namespace CodeverseWPF.DB;

public partial class ReportEmployeeOrderCount
{
    public DateOnly? Date { get; set; }

    public int? Count { get; set; }

    public string Name { get; set; } = null!;

    public string Surname { get; set; } = null!;
}
