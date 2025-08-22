using System.Windows;
using System.Windows.Controls;
using CodeverseWPF.DB;

namespace CodeverseWPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowCheck.xaml
    /// </summary>
    public partial class WindowCheck : Window
    {
        int? _orderId;
        public WindowCheck(int orderId)
        {
            InitializeComponent();
            _orderId = orderId;
            using (CodeverseContext db = new CodeverseContext())
            {
                TBDateCheckPay.Text = $"Дата оплаты - {DateTime.Now}";
                var order = db.ViewOrders
                    .FirstOrDefault(p => p.OrderId == _orderId);
                if (order != null)
                {
                    TBOrderDateStart.Text = $"Дата приема - {order.Date}";
                    TBOrderID.Text = $"# {order.OrderId}";
                    TBSum.Text = $"ИТОГО {order.Total} Руб.";
                    var client = db.Clients
                        .FirstOrDefault(p => p.ClientId == order.ClientId);
                    if (client != null)
                    {
                        TBClientName.Text = $"{client.Surname} {client.Name}";
                        TBClientEmail.Text = client.Email;
                        TBClientPhone.Text = client.Phone;
                    }
                    var orderEmployee = db.ViewOrderEmployees
                        .FirstOrDefault(p => p.OrderId == order.OrderId);
                    if(orderEmployee != null)
                    {
                        var employee = db.ViewEmployees
                            .FirstOrDefault(p => p.EmployeeId == orderEmployee.EmployeeId);
                        if(employee != null)
                        {
                            TBMasterName.Text = $"Мастер - {employee.Surname} {employee.Name}";
                            TBMasterPosition.Text = $"Должность - {employee.Position}";
                        }
                    }

                    var completedOrder = db.CompletedOrders
                        .FirstOrDefault(p => p.OrderId == order.OrderId);
                    if (completedOrder != null)
                    {
                        TBOrderDateEnd.Text = $"Дата выполнения - {completedOrder.Date}";
                    }

                    var orderDevices = from p in db.ViewOrderDevices
                        .Where(p => p.OrderId == order.OrderId)
                        select new { Компьютер = p.Device, 
                            Цена = p.Price.ToString() + " Руб.", 
                            Количество = p.Count, 
                            Итого = (p.Price * p.Count).ToString() + " Руб."};
                    DGItems.ItemsSource = orderDevices.ToList();

                    var orderServices = from p in db.ViewOrderServices
                        .Where(p => p.OrderId == order.OrderId)
                        select new { Услуга = p.Service, 
                            Цена = p.Price.ToString() + " Руб."};
                    DGServices.ItemsSource = orderServices.ToList();

                    var details = from p in db.ViewOrderDetails
                        .Where(p => p.OrderId == order.OrderId)
                        select new { Деталь = p.Detail, 
                            Цена = p.Price.ToString() + " Руб.", 
                            Количество = p.Count, 
                            Итого = (p.Price * p.Count).ToString() + " Руб."};
                    DGDetails.ItemsSource = details.ToList();
                }
            }
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog dialog = new PrintDialog();
            if (dialog.ShowDialog() == true)
            {
                dialog.PrintVisual(WPMain, "Товарный чек");
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
