using CodeverseWPF.DB;
using System.Windows;

namespace CodeverseWPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowShowInfo.xaml
    /// </summary>
    public partial class WindowShowInfo : Window
    {
        int? _orderId;
        public WindowShowInfo(int orderID)
        {
            InitializeComponent();
            _orderId = orderID;    
            Refresh();
        }

        private void Refresh()
        {
            if (_orderId != null)
            {
                using (CodeverseContext db = new CodeverseContext())
                {
                    var devices = db.ViewOrderDevices
                        .Where(p => p.OrderId == _orderId);
                    ListDevices.ItemsSource = devices.ToList();

                    var details = db.ViewOrderDetails
                        .Where(p => p.OrderId == _orderId);
                    ListDetails.ItemsSource = details.ToList();

                    var services = db.ViewOrderServices
                        .Where(p => p.OrderId == _orderId);
                    ListServices.ItemsSource = services.ToList();

                    var order = db.ViewOrders.FirstOrDefault(p => p.OrderId == _orderId);
                    if (order != null)
                    {
                        TBClient.Text = $"{order.Surname} {order.Name}";
                        TBDate.Text = $"{order.Date}";
                        TBOrderID.Text = $"{_orderId}";
                        TBState.Text = order.State;
                        TBTotal.Text = $"{order.Total} Руб.";
                    }
                }
            }
        }

        private void ListDetails_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(_orderId != null)
            {
                WindowOrderDetails windowOrderDetails = new WindowOrderDetails((int)_orderId);
                windowOrderDetails.ShowDialog();
                Refresh();
            }
        }

        private void ListServices_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (_orderId != null)
            {
                WindowOrderServices windowOrderServices = new WindowOrderServices((int)_orderId);
                windowOrderServices.ShowDialog();
                Refresh();
            }
        }
    }
}
