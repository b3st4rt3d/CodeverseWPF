using System.Windows;
using System.Windows.Controls;
using CodeverseWPF.DB;

namespace CodeverseWPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowOrderDevices.xaml
    /// </summary>
    public partial class WindowOrderDevices : Window
    {
        int? _orderId;
        public WindowOrderDevices(int orderId)
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
                    if(_orderId != null)
                    {
                        var devices = db.Devices;
                        ListDevices.ItemsSource = devices.ToList();

                        var orderDevices = db.ViewOrderDevices
                            .Where(p => p.OrderId == (int)_orderId);
                        ListOrderEmployee.ItemsSource = orderDevices.ToList();
                    }
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
                    var devices = db.Devices
                        .Where(p => p.Device1.ToLower().Contains(searchString)
                        || p.Price.ToString().Contains(searchString));
                    ListDevices.ItemsSource = devices.ToList();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
            TBSearch.Clear();
        }

        private void ListDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var device = ListDevices.SelectedItem as Device;
                if(device != null && _orderId != null)
                {
                    using(CodeverseContext db = new CodeverseContext())
                    {
                        var details = db.Configs
                            .Where(p => p.DeviceId == device.DeviceId).ToList();

                        foreach (var i in details)
                        {
                            var detail = db.Details.FirstOrDefault(p => p.DetailId == i.DetailId);
                            if (detail != null)
                            {
                                if (detail.Count < i.Count)
                                {
                                    string sMessageBoxText = "Недостатосное количетсво деталей";
                                    string sCaption = "Хотите ли вы запросить недостаточное количество деталей?";

                                    MessageBoxButton btnMessageBox = MessageBoxButton.YesNo;
                                    MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                                    MessageBoxResult rsltMessageBox = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);

                                    if (rsltMessageBox == MessageBoxResult.Yes) MakeRequestDetails(detail, (int)(i.Count - detail.Count));
                                    else return;
                                }
                            }
                        }

                        foreach (var i in details)
                        {
                            var detail = db.Details.FirstOrDefault(p => p.DetailId == i.DetailId);
                            if (detail != null)
                            {
                                if (detail.Count >= i.Count) detail.Count -= i.Count;
                            }
                        }

                        var unique = db.OrderDevices
                            .FirstOrDefault(p => p.OrderId == (int)_orderId
                            && p.DeviceId == device.DeviceId);

                        if (unique != null) unique.Count++;
                        else
                        {
                            OrderDevice orderDevice = new OrderDevice();
                            orderDevice.DeviceId = device.DeviceId;
                            orderDevice.OrderId = (int)_orderId;
                            orderDevice.Count = 1;

                            db.OrderDevices.Add(orderDevice);
                        }

                        var order = db.Orders
                            .FirstOrDefault(p => p.OrderId == (int)_orderId);
                        if (order != null) order.Total += device.Price;

                        db.SaveChanges();
                        Refresh();
                    }
                }
            }
            catch(Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void ListOrderEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var orderDevices = ListOrderEmployee.SelectedItem as ViewOrderDevice;
                if (orderDevices != null && _orderId != null)
                {
                    using (CodeverseContext db = new CodeverseContext())
                    {
                        var details = db.Configs
                            .Where(p => p.DeviceId == orderDevices.DeviceId).ToList();
                        foreach (var i in details)
                        {
                            var detail = db.Details.FirstOrDefault(p => p.DetailId == i.DetailId);
                            if(detail != null) detail.Count += i.Count;
                        }

                        var unique = db.OrderDevices
                            .FirstOrDefault(p => p.OrderId == (int)_orderId
                            && p.DeviceId == orderDevices.DeviceId);

                        if (unique != null)
                        {
                            if(unique.Count != 1) unique.Count--;
                            else db.OrderDevices.Remove(unique);
                        }

                        var order = db.Orders
                            .FirstOrDefault(p => p.OrderId == (int)_orderId);
                        if (order != null) order.Total -= orderDevices.Price;

                        db.SaveChanges();
                        Refresh();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void MakeRequestDetails(Detail _detail, int count)
        {
            try
            {
                if (_detail != null && _orderId != null)
                {

                    using (CodeverseContext db = new CodeverseContext())
                    {
                        Request detailRequest = new Request
                        {
                            OrderId = _orderId,
                            DetailId = _detail.DetailId,
                            Count = count,
                            Date = DateTime.Now,
                        };
                        db.Requests.Add(detailRequest);

                        var selectedOrder = db.Orders
                            .FirstOrDefault(o => o.OrderId == _orderId);

                        if (selectedOrder != null) selectedOrder.StateId = 2;

                        MessageBox.Show($"В заказ №{_orderId} быда запрошена деталь {_detail.Detail1} в количестве {count}");

                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}
