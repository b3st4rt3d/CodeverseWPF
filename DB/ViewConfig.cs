using System;
using System.Collections.Generic;

namespace CodeverseWPF.DB;

public partial class ViewConfig
{
    public int ConfigId { get; set; }

    public int? DeviceId { get; set; }

    public int? DetailId { get; set; }

    public int Count { get; set; }

    public string Device { get; set; } = null!;

    public decimal Price { get; set; }

    public string Detail { get; set; } = null!;

    public decimal? DetailPrice { get; set; }

    public string? Description { get; set; }

    public int BrandId { get; set; }

    public string Brand { get; set; } = null!;

    public int TypeId { get; set; }

    public string Type { get; set; } = null!;

    public byte[]? Image { get; set; }

    public byte[]? DetailImage { get; set; }
}
