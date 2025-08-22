using System;
using System.Collections.Generic;

namespace CodeverseWPF.DB;

public partial class ViewOrderService
{
    public int OrderId { get; set; }

    public int ServiceId { get; set; }

    public string Service { get; set; } = null!;

    public decimal Price { get; set; }
}
