using System;
using System.Collections.Generic;

namespace CodeverseWPF.DB;

public partial class Device
{
    public int DeviceId { get; set; }

    public string Device1 { get; set; } = null!;

    public decimal Price { get; set; }

    public byte[]? Image { get; set; }

    public virtual ICollection<Config> Configs { get; set; } = new List<Config>();

    public virtual ICollection<OrderDevice> OrderDevices { get; set; } = new List<OrderDevice>();
}
