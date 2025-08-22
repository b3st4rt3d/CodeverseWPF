using System;
using System.Collections.Generic;

namespace CodeverseWPF.DB;

public partial class ReportPopularService
{
    public DateOnly? Date { get; set; }

    public int? Count { get; set; }

    public string Service { get; set; } = null!;

    public decimal Price { get; set; }
}
