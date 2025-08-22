using System;
using System.Collections.Generic;

namespace CodeverseWPF.DB;

public partial class State
{
    public int StateId { get; set; }

    public string State1 { get; set; } = null!;

    public string? Background { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
