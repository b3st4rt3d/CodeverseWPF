using System;
using System.Collections.Generic;

namespace CodeverseWPF.DB;

public partial class OrderDevice
{
    public int OrderDeviceId { get; set; }

    public int OrderId { get; set; }

    public int DeviceId { get; set; }

    public int Count { get; set; }

    public virtual Device Device { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
