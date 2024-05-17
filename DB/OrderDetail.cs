using System;
using System.Collections.Generic;

namespace CodeverseWPF;

public partial class OrderDetail
{
    public int OrderDetailId { get; set; }

    public int OrderId { get; set; }

    public int DetailId { get; set; }

    public virtual Detail Detail { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
