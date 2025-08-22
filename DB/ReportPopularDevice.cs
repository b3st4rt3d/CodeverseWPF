using System;
using System.Collections.Generic;

namespace CodeverseWPF.DB;

public partial class ReportPopularDevice
{
    public DateOnly? Date { get; set; }

    public int? Count { get; set; }

    public string Device { get; set; } = null!;
}
