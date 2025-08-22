using System;
using System.Collections.Generic;

namespace CodeverseWPF.DB;

public partial class ViewOrderDevice
{
    public int OrderDeviceId { get; set; }

    public int OrderId { get; set; }

    public int DeviceId { get; set; }

    public string Device { get; set; } = null!;

    public decimal Price { get; set; }

    public byte[]? Image { get; set; }

    public int Count { get; set; }
}
