using System.Windows;
using CodeverseWPF.DB;

namespace CodeverseWPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowOrderServices.xaml
    /// </summary>
    public partial class WindowOrderServices : Window
    {
        int? _orderId;
        public WindowOrderServices(int orderId)
        {
            InitializeComponent();
            _orderId = orderId;
            Refresh();
        }

        private void Refresh()
        {
            try
            {
                using (CodeverseContext db = new CodeverseContext())
                {
                    var services = db.Services;
                    ListServices.ItemsSource = services.ToList();

                    var orderServices = db.ViewOrderServices
                        .Where(p => p.OrderId == _orderId);
                    ListOrderServices.ItemsSource = orderServices.ToList();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var searchString = TBSearch.Text.Trim().ToLower();
                using (CodeverseContext db = new CodeverseContext())
                {
                    var services = db.Services
                        .Where(p => p.Service1.ToLower().Contains(searchString)
                        || p.Price.ToString().Contains(searchString));
                    ListServices.ItemsSource = services.ToList();
                }
            }
            catch(Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void ListServices_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                var selectedService = ListServices.SelectedItem as Service;
                if(selectedService != null)
                {
                    using (CodeverseContext db = new CodeverseContext())
                    {
                        var unique = db.OrderServices
                            .FirstOrDefault(p => p.OrderId == _orderId
                            && p.ServiceId == selectedService.ServiceId);
                        if(unique == null && _orderId != null)
                        {
                            var order = db.Orders
                                .FirstOrDefault(p => p.OrderId == _orderId);
                            if(order != null) order.Total += selectedService.Price;

                            OrderService orderService = new OrderService { 
                                OrderId = (int)_orderId,
                                ServiceId = selectedService.ServiceId,
                            };

                            db.OrderServices.Add(orderService);
                            db.SaveChanges();
                            Refresh();
                        }
                    }
                }
            }
            catch(Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void ListOrderServices_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                var selectedService = ListServices.SelectedItem as ViewOrderService;
                if (selectedService != null)
                {
                    using (CodeverseContext db = new CodeverseContext())
                    {
                        var orderServices = db.OrderServices
                            .FirstOrDefault(p => p.OrderId == _orderId
                            && p.ServiceId == selectedService.ServiceId);

                        var order = db.Orders
                            .FirstOrDefault(p => p.OrderId == _orderId);
                        if (order != null) order.Total -= selectedService.Price;

                        if (orderServices != null)
                        {
                            db.OrderServices.Remove(orderServices);
                            db.SaveChanges();
                            Refresh();
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}
