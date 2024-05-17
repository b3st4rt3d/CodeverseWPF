using System;
using System.Collections.Generic;

namespace CodeverseWPF;

public partial class Service
{
    public int ServiceId { get; set; }

    public string Service1 { get; set; } = null!;

    public decimal Price { get; set; }

    public virtual ICollection<OrderService> OrderServices { get; set; } = new List<OrderService>();
}
