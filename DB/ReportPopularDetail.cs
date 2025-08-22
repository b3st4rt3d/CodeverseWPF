using System;
using System.Collections.Generic;

namespace CodeverseWPF.DB;

public partial class ReportPopularDetail
{
    public DateOnly? Date { get; set; }

    public string Detail { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string Brand { get; set; } = null!;

    public int? Count { get; set; }
}
