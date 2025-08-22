using System.Windows;
using CodeverseWPF.DB;

namespace CodeverseWPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowCreateOrder.xaml
    /// </summary>
    public partial class WindowCreateOrder : Window
    {
        Client? _selectedClient;
        int? _orderId;
        public WindowCreateOrder(int orderId)
        {
            InitializeComponent();
            Refresh();
            DatePicker.SelectedDate = DateTime.Now;
            _orderId = orderId;
            if(orderId != 0)
            {
                Create.Content = "Сохранить";
                Return.Content = "Отмена";
            }
        }

        private void Refresh()
        {
            try { 
                using (CodeverseContext db = new CodeverseContext())
                {
                    var clients = db.Clients;
                    ListClient.ItemsSource = clients.ToList();
                }
            }
            catch(Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedClient != null && _orderId != null)
                {
                    if (DatePicker.SelectedDate != null)
                    {
                        if(_orderId == 0)
                        {
                            Order order = new Order
                            {
                                Date = DateOnly.FromDateTime((DateTime)DatePicker.SelectedDate),
                                ClientId = _selectedClient.ClientId,
                                StateId = 1,
                                Total = 0
                            };
                            using (CodeverseContext db = new CodeverseContext())
                            {
                                db.Orders.Add(order);
                                db.SaveChanges();
                                var lastOrder = db.ViewOrders.ToList();
                                foreach (var item in lastOrder) _orderId = item.OrderId;
                            }
                        }
                        else
                        {
                            using (CodeverseContext db = new CodeverseContext())
                            {
                                var order = db.Orders
                                    .FirstOrDefault(p => p.OrderId == _orderId);
                                if (order != null)
                                {
                                    order.ClientId = _selectedClient.ClientId;
                                    order.Date = DateOnly.FromDateTime((DateTime)DatePicker.SelectedDate);
                                    db.SaveChanges();
                                }
                            }
                        }
                        SPClient.Visibility = Visibility.Collapsed;
                        SPOptions.Visibility = Visibility.Visible;
                        Create.Visibility = Visibility.Collapsed;
                        Return.Margin = new System.Windows.Thickness(0, 0, 0, 0);
                        Return.Content = "Выйти";
                    }
                    else MessageBox.Show("Выберите дату.");
                }
                else MessageBox.Show("Выберите клиента.");
            }
            catch(Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void Return_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var searchString = TBSearch.Text.Trim().ToLower();
                using (CodeverseContext db = new CodeverseContext())
                {
                    var clients = db.Clients
                        .Where(p => p.Name.Trim().ToLower().Contains(searchString)
                        || p.Name.Trim().ToLower().Contains(searchString)
                        || p.Surname.Trim().ToLower().Contains(searchString)
                        || p.Email.Trim().ToLower().Contains(searchString)
                        || p.Phone.Trim().ToLower().Contains(searchString));

                    ListClient.ItemsSource = clients.ToList();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
            TBSearch.Clear();
        }

        private void ListClient_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _selectedClient = ListClient.SelectedItem as Client;
        }

        private void BtnServices_Click(object sender, RoutedEventArgs e)
        {
            if(_orderId != null)
            {
                WindowOrderServices windowOrderServices = new WindowOrderServices((int)_orderId);
                windowOrderServices.ShowDialog();
            }
        }

        private void BtnDevices_Click(object sender, RoutedEventArgs e)
        {
            if (_orderId != null)
            {
                WindowOrderDevices windowOrderDevices = new WindowOrderDevices((int)_orderId);
                windowOrderDevices.ShowDialog();
            }
        }

        private void BtnDetails_Click(object sender, RoutedEventArgs e)
        {
            if (_orderId != null)
            {
                WindowOrderDetails windowOrderDetails = new WindowOrderDetails((int)_orderId);
                windowOrderDetails.ShowDialog();
            }
        }
    }
}
