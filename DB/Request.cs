using System;
using System.Collections.Generic;

namespace CodeverseWPF.DB;

public partial class Request
{
    public int RequestId { get; set; }

    public int? OrderId { get; set; }

    public int? DetailId { get; set; }

    public DateTime? Date { get; set; }

    public int? Count { get; set; }

    public virtual Detail? Detail { get; set; }

    public virtual Order? Order { get; set; }
}
