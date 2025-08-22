using System;
using System.Collections.Generic;

namespace CodeverseWPF.DB;

public partial class CompletedOrder
{
    public int CompletedOrderId { get; set; }

    public int? OrderId { get; set; }

    public DateOnly? Date { get; set; }

    public virtual Order? Order { get; set; }
}
