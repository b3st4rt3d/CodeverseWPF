using System;
using System.Collections.Generic;

namespace CodeverseWPF;

public partial class Brand
{
    public int BrandId { get; set; }

    public string Brand1 { get; set; } = null!;

    public virtual ICollection<Detail> Details { get; set; } = new List<Detail>();
}
