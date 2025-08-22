using System;
using System.Collections.Generic;

namespace CodeverseWPF.DB;

public partial class Order
{
    public int OrderId { get; set; }

    public int? ClientId { get; set; }

    public int? StateId { get; set; }

    public DateOnly? Date { get; set; }

    public decimal? Total { get; set; }

    public virtual Client? Client { get; set; }

    public virtual ICollection<CompletedOrder> CompletedOrders { get; set; } = new List<CompletedOrder>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<OrderDevice> OrderDevices { get; set; } = new List<OrderDevice>();

    public virtual ICollection<OrderEmployee> OrderEmployees { get; set; } = new List<OrderEmployee>();

    public virtual ICollection<OrderService> OrderServices { get; set; } = new List<OrderService>();

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public virtual State? State { get; set; }
}
