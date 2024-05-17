using System;
using System.Collections.Generic;

namespace CodeverseWPF;

public partial class ViewDetail
{
    public int DetailId { get; set; }

    public string Detail { get; set; } = null!;

    public decimal? Price { get; set; }

    public string? Description { get; set; }

    public int TypeId { get; set; }

    public string Type { get; set; } = null!;

    public int BrandId { get; set; }

    public string Brand { get; set; } = null!;

    public byte[]? Image { get; set; }
}
