using System;
using System.Collections.Generic;

namespace CodeverseWPF;

public partial class Config
{
    public int ConfigId { get; set; }

    public int? DeviceId { get; set; }

    public int? DetailId { get; set; }

    public int Count { get; set; }

    public virtual Detail? Detail { get; set; }

    public virtual Device? Device { get; set; }
}
