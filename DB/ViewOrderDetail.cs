using System;
using System.Collections.Generic;

namespace CodeverseWPF.DB;

public partial class ViewOrderDetail
{
    public int DetailId { get; set; }

    public string Detail { get; set; } = null!;

    public decimal? Price { get; set; }

    public string? Description { get; set; }

    public byte[]? Image { get; set; }

    public int TypeId { get; set; }

    public string Type { get; set; } = null!;

    public int BrandId { get; set; }

    public string Brand { get; set; } = null!;

    public int OrderId { get; set; }

    public int? Count { get; set; }
}
