using System;
using System.Collections.Generic;

namespace CodeverseWPF;

public partial class Detail
{
    public int DetailId { get; set; }

    public string Detail1 { get; set; } = null!;

    public int? TypeId { get; set; }

    public int? BrandId { get; set; }

    public decimal? Price { get; set; }

    public string? Description { get; set; }

    public byte[]? Image { get; set; }

    public virtual Brand? Brand { get; set; }

    public virtual ICollection<Config> Configs { get; set; } = new List<Config>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Type? Type { get; set; }
}
