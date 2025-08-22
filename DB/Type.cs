using System;
using System.Collections.Generic;

namespace CodeverseWPF.DB;

public partial class Type
{
    public int TypeId { get; set; }

    public string Type1 { get; set; } = null!;

    public virtual ICollection<Detail> Details { get; set; } = new List<Detail>();
}
