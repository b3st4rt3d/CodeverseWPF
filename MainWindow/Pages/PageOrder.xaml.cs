using CodeverseWPF.DB;
using CodeverseWPF.Windows;
using System.Windows;
using System.Windows.Controls;

namespace CodeverseWPF.MainWindow.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageOrder.xaml
    /// </summary>
    public partial class PageOrder : Page
    {
        ViewOrder? _selectedOrder;
        bool showAll = false;
        public PageOrder()
        {
            InitializeComponent();
            Refresh();
        }

        private void Refresh()
        {
            try
            {
                using (CodeverseContext db = new CodeverseContext())
                {
                    if(showAll)
                    {
                        var orders = db.ViewOrders;
                        ListOrder.ItemsSource = orders.ToList();
                    }
                    else
                    {
                        var orders = db.ViewOrders
                            .Where(p => p.StateId != 7);
                        ListOrder.ItemsSource = orders.ToList();
                    }

                    var states = db.States
                        .OrderBy(p => p.StateId);
                    CBState.ItemsSource = states.ToList();
                    CBStateFilter.ItemsSource = states.ToList();
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
                    var orders = db.ViewOrders
                        .Where(p => p.Name.Trim().ToLower().Contains(searchString)
                        || p.Surname.Trim().ToLower().Contains(searchString)
                        || p.OrderId.ToString().Contains(searchString)
                        || p.State.Trim().ToLower().Contains(searchString)
                        || p.Date.ToString().Contains(searchString));
                    ListOrder.ItemsSource = orders.ToList();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

         private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
            TBSearch.Clear();
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (CodeverseContext db = new CodeverseContext())
                {
                    int lastIndex = db.ViewOrders.Count();
                    WindowCreateOrder wco = new WindowCreateOrder(0);
                    wco.ShowDialog();

                    int nextIndex = db.ViewOrders.Count();
                    if (lastIndex != nextIndex) ResentAct.Text = $"Создан новый заказ №{lastIndex}";                    
                    Refresh();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnChange_Click(object sender, RoutedEventArgs e)
        {
            if(_selectedOrder!= null)
            {
                WindowCreateOrder windowCreateOrder = new WindowCreateOrder(_selectedOrder.OrderId);
                windowCreateOrder.ShowDialog();
                ResentAct.Text = $"Обновлен заказ {_selectedOrder.OrderId}";
                Refresh();
                ClearTB();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Если вы удалите заказ, его нельзя будет восстановить. Удалить заказ?",
                    "Удаление заказа",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (_selectedOrder != null)
                    {
                        using (CodeverseContext db = new CodeverseContext())
                        {
                            var order = db.Orders
                                .FirstOrDefault(p => p.OrderId == _selectedOrder.OrderId);

                            var orderDetails = db.OrderDetails
                                .Where(p => p.OrderId == _selectedOrder.OrderId);

                            var orderDevices = db.OrderDevices
                                .Where(p => p.OrderId == _selectedOrder.OrderId);

                            var orderServices = db.OrderServices
                                .Where(p => p.OrderId == _selectedOrder.OrderId);

                            var completedOrder = db.CompletedOrders
                                .FirstOrDefault(p => p.OrderId == _selectedOrder.OrderId);

                            if (order != null)
                            {
                                if(completedOrder != null) db.CompletedOrders.Remove(completedOrder);

                                foreach(var orderDetail in orderDetails)
                                {
                                    var detail = db.Details
                                        .FirstOrDefault(p => p.DetailId == orderDetail.DetailId);
                                    if (detail != null) detail.Count += orderDetail.Count;
                                    db.OrderDetails.Remove(orderDetail);

                                }
                                foreach(var orderDevice in orderDevices)
                                {
                                    var details = db.Configs
                                        .Where(p => p.DeviceId == orderDevice.DeviceId);
                                    foreach (var item in details)
                                    {
                                        var detail = db.Details
                                            .FirstOrDefault(p => p.DetailId == item.DetailId);
                                        if (detail != null) detail.Count += item.Count;
                                    }
                                    db.OrderDevices.Remove(orderDevice);
                                }
                                foreach(var orderService in orderServices) db.OrderServices.Remove(orderService);

                                db.Orders.Remove(order);
                                db.SaveChanges();
                                ResentAct.Text = $"Удален заказ {_selectedOrder.OrderId}";
                                Refresh();
                            }
                            else MessageBox.Show("Выберите заказ");
                        }
                    }
                    else { MessageBox.Show("Выберите заказ"); }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnCompleteChange_Click(object sender, RoutedEventArgs e)
        {
            SPChangeBtns.Visibility = Visibility.Collapsed;
            SPFilterBtns.IsEnabled = true;
            SPMainBtns.Visibility = Visibility.Visible;
            ClearTB();
        }

        private void BtnReturnChange_Click(object sender, RoutedEventArgs e)
        {
            SPChangeBtns.Visibility = Visibility.Collapsed;
            SPFilterBtns.IsEnabled = true;
            SPMainBtns.Visibility = Visibility.Visible;
            ClearTB();
        }

        private void BtnChangeDevices_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedOrder != null)
            {
                WindowOrderDevices wod = new WindowOrderDevices(_selectedOrder.OrderId);
                wod.ShowDialog();
                Refresh();
                ClearTB();
            }
        }

        private void ListOrder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedOrder = ListOrder.SelectedItem as ViewOrder;
            if (_selectedOrder != null)
            {
                BtnChange.IsEnabled = true;
                BtnDelete.IsEnabled = true;
                BtnCheck.IsEnabled = true;
                TBClient.Text = _selectedOrder.Name + " " + _selectedOrder.Surname;
                TBOrderID.Text = _selectedOrder.OrderId.ToString();
                TBTotal.Text  = _selectedOrder.Total.ToString();
                CBState.SelectedIndex = _selectedOrder.StateId - 1;
                DPDate.SelectedDate = DateTime.Parse(_selectedOrder.Date.ToString());
                using (CodeverseContext db = new CodeverseContext())
                {
                    var devices = db.ViewOrderDevices
                        .Where(p => p.OrderId == _selectedOrder.OrderId);
                    ListDevices.ItemsSource = devices.ToList();  

                    var services = db.ViewOrderServices
                        .Where(p => p.OrderId == _selectedOrder.OrderId);
                    ListServices.ItemsSource = services.ToList();

                    var details = db.ViewOrderDetails
                        .Where(p => p.OrderId == _selectedOrder.OrderId);
                    ListDetails.ItemsSource = details.ToList();
                }
            }
        }

        private void CBStateFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var state = CBStateFilter.SelectedItem as State;
                if (state != null)
                {
                    using (CodeverseContext db = new CodeverseContext())
                    {
                        var orders = db.ViewOrders
                            .Where(p => p.StateId == state.StateId);
                        ListOrder.ItemsSource = orders.ToList();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void ClearTB()
        {
            ListDevices.ItemsSource = null;
            ListDetails.ItemsSource = null;
            ListServices.ItemsSource = null;
            TBClient.Clear();
            TBOrderID.Clear();
            TBTotal.Clear();
            CBState.SelectedIndex = -1;
            DPDate.SelectedDate = null;
        }

        private void BtnCheck_Click(object sender, RoutedEventArgs e)
        {
            if(_selectedOrder != null)
            {
                if(_selectedOrder.StateId != 7)
                {
                    MessageBox.Show("Чтобы создать чек, завершите заказ.");
                    return;
                }
                WindowCheck windowCheck = new WindowCheck(_selectedOrder.OrderId);
                windowCheck.ShowDialog();
            }
        }

        private void BtnService_Click(object sender, RoutedEventArgs e)
        {
            SPServices.Visibility = Visibility.Visible;
            SPDevices.Visibility = Visibility.Collapsed;
            SPDetails.Visibility = Visibility.Collapsed;
            BtnService.IsEnabled = false;
            BtnDevices.IsEnabled = true;
            BtnDetails.IsEnabled = true;
        }

        private void BtnDevices_Click(object sender, RoutedEventArgs e)
        {
            SPServices.Visibility = Visibility.Collapsed;
            SPDevices.Visibility = Visibility.Visible;
            SPDetails.Visibility = Visibility.Collapsed;
            BtnService.IsEnabled = true;
            BtnDevices.IsEnabled = false;
            BtnDetails.IsEnabled = true;
        }

        private void BtnDetails_Click(object sender, RoutedEventArgs e)
        {
            SPServices.Visibility = Visibility.Collapsed;
            SPDevices.Visibility = Visibility.Collapsed;
            SPDetails.Visibility = Visibility.Visible;
            BtnService.IsEnabled = true;
            BtnDevices.IsEnabled = true;
            BtnDetails.IsEnabled = false;
        }

        private void BtnChangeService_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedOrder != null)
            {
                WindowOrderServices windowOrderServices = new WindowOrderServices(_selectedOrder.OrderId);
                windowOrderServices.ShowDialog();
                ClearTB();
                Refresh();
            }
        }

        private void BtnChangeDetails_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedOrder != null)
            {
                WindowOrderDetails windowOrderDetails = new WindowOrderDetails(_selectedOrder.OrderId);
                windowOrderDetails.ShowDialog();
                ClearTB();
                Refresh();
            }
        }

        private void BtnServices_Click(object sender, RoutedEventArgs e)
        {
            WindowServices windowServices = new WindowServices();
            windowServices.ShowDialog();
        }

        private void BtnShowAll_Click(object sender, RoutedEventArgs e)
        {
            showAll = !showAll;
            BtnShowAll.Content = showAll ? "Скрыть закрытые" : "Показать скрытые";
            Refresh();
        } 
    }
}
