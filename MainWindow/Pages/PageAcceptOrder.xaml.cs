using CodeverseWPF.DB;
using CodeverseWPF.Windows;
using System.Windows;
using System.Windows.Controls;

namespace CodeverseWPF.MainWindow.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageAcceptOrder.xaml
    /// </summary>
    public partial class PageAcceptOrder : Page
    {
        private Employee? _employee;
        private ViewOrder? _selectedOrder;
        public PageAcceptOrder(Employee user)
        {
            InitializeComponent();
            _employee = user;
            Refresh();
            CheckLatestOrders();
            BtnShowClient.IsEnabled = false;
            BtnShowInfo.IsEnabled = false;
            if (user.PositionId != 4)
            {
                BtnWaitClietOrder.Visibility = Visibility.Collapsed;
                BtnIssuedOrder.Visibility = Visibility.Collapsed;
            }
            if (user.PositionId != 4 && user.PositionId != 2)
            {
                BtnShowClient.Visibility = Visibility.Collapsed;
            }
            if (user.PositionId != 3)
            {
                BtnAcceptOrder.Visibility = Visibility.Collapsed;
                BtnCompleteOrder.Visibility = Visibility.Collapsed;
                BtnRefusalOrder.Visibility = Visibility.Collapsed;
            }
            if (user.PositionId == 4)
            {
                TBLeft.Text = "Готовые заказы";
                TBCenter.Text = "Ожидают клиента";
                TBRight.Text = "Выданные заказы";
            }
        }

        private void Refresh()
        {
            try
            {
                if (_employee != null)
                {
                    using (CodeverseContext db = new CodeverseContext())
                    {
                        if (_employee.PositionId == 2)
                        {
                            var orders = db.ViewOrders;

                            var NewOrders = from p in orders
                                            where p.StateId == 1
                                            select p;

                            var AcceptOrders = from p in orders
                                               where p.StateId == 2 || p.StateId == 3 || p.StateId == 5 || p.StateId == 6
                                               select p;

                            var CompleteOrders = from p in orders
                                                 where p.StateId == 4 || p.StateId == 7
                                                 orderby p.Date descending
                                                 select p;

                            ListNewOrder.ItemsSource = NewOrders.ToList();
                            ListAcceptOrder.ItemsSource = AcceptOrders.ToList();
                            ListCompleteOrder.ItemsSource = CompleteOrders.ToList();
                        }
                        if (_employee.PositionId == 3)
                        {
                            var orders = db.ViewOrders;

                            var NewOrders = from p in orders
                                            where p.StateId == 1
                                            select p;

                            var AcceptOrders = from p in db.ViewOrderEmployees
                                               where (p.StateId == 2 || p.StateId == 3)
                                               && p.EmployeeId == _employee.EmployeeId
                                               select p;

                            var CompleteOrders = from p in db.ViewOrderEmployees
                                                 where p.StateId == 4
                                                 && p.EmployeeId == _employee.EmployeeId
                                                 select p;

                            ListNewOrder.ItemsSource = NewOrders.ToList();
                            ListAcceptOrder.ItemsSource = AcceptOrders.ToList();
                            ListCompleteOrder.ItemsSource = CompleteOrders.ToList();
                        }
                        if (_employee.PositionId == 4)
                        {
                            DateTime lastWeek = DateTime.Now.AddDays(-7);
                            var orders = db.ViewOrders;

                            var NewOrders = from p in orders
                                            where p.StateId == 4
                                            select p;

                            var AcceptOrders = from p in orders
                                               where p.StateId == 5 || p.StateId == 6
                                               select p;

                            var CompleteOrders = from p in orders
                                                 where p.StateId == 7
                                                 && p.Date > DateOnly.FromDateTime(lastWeek)
                                                 orderby p.Date descending
                                                 select p;

                            ListNewOrder.ItemsSource = NewOrders.ToList();
                            ListAcceptOrder.ItemsSource = AcceptOrders.ToList();
                            ListCompleteOrder.ItemsSource = CompleteOrders.ToList();
                        }
                    }

                }
                else MessageBox.Show("Извините, произошла ошибка");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnAcceptOrder_Click(object sender, RoutedEventArgs e)
        {
            ChangeOrderState(ListNewOrder, 3);
        }

        private void BtnRefusalOrder_Click(object sender, RoutedEventArgs e)
        {
            ChangeOrderState(ListCompleteOrder, 1);
            ChangeOrderState(ListAcceptOrder, 1);
        }

        private void BtnCompleteOrder_Click(object sender, RoutedEventArgs e)
        {
            ChangeOrderState(ListAcceptOrder, 4);
        }

        private void BtnShowClient_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedOrder != null)
            {
                var showContact = new ContactInformation(_selectedOrder.OrderId);
                showContact.ShowDialog();
            }
            else MessageBox.Show("Выберите заказ");
        }

        private void BtnWaitClietOrder_Click(object sender, RoutedEventArgs e)
        {
            ChangeOrderState(ListNewOrder, 6);
        }

        private void BtnIssuedOrder_Click(object sender, RoutedEventArgs e)
        {
            ChangeOrderState(ListAcceptOrder, 7);
        }

        private void ListNewOrder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListAcceptOrder.SelectedItem = null;
            ListCompleteOrder.SelectedItem = null;
            CheckSelectedItem();
            _selectedOrder = ListNewOrder.SelectedItem as ViewOrder;
        }

        private void ListAcceptOrder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListNewOrder.SelectedItem = null;
            ListCompleteOrder.SelectedItem = null;
            CheckSelectedItem();
            _selectedOrder = ListAcceptOrder.SelectedItem as ViewOrder;
        }

        private void ListCompleteOrder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListAcceptOrder.SelectedItem = null;
            ListNewOrder.SelectedItem = null;
            CheckSelectedItem();
            _selectedOrder = ListCompleteOrder.SelectedItem as ViewOrder;
        }

        public void CheckSelectedItem()
        {
            BtnAcceptOrder.IsEnabled = false;
            BtnRefusalOrder.IsEnabled = false;
            BtnCompleteOrder.IsEnabled = false;
            BtnWaitClietOrder.IsEnabled = false;
            BtnIssuedOrder.IsEnabled = false;
            BtnShowInfo.IsEnabled = true;
            BtnShowClient.IsEnabled = true;


            if (ListNewOrder.SelectedItem != null)
            {
                BtnAcceptOrder.IsEnabled = true;
                BtnWaitClietOrder.IsEnabled = true;
                return;
            }
            if (ListCompleteOrder.SelectedItem != null)
            {
                BtnRefusalOrder.IsEnabled = true;
                return;
            }
            if (ListAcceptOrder.SelectedItem != null)
            {
                BtnCompleteOrder.IsEnabled = true;
                BtnRefusalOrder.IsEnabled = true;
                BtnIssuedOrder.IsEnabled = true;
                return;
            }
        }

        private void ChangeOrderState(ListBox list, int stateId)
        {
            try
            {
                int? orderId;
                if (_employee?.PositionId == 3
                    && list != ListNewOrder)
                {
                    var selectedOrder = list.SelectedItem as ViewOrderEmployee;
                    orderId = selectedOrder?.OrderId;
                }
                else
                {
                    var selectedOrder = list.SelectedItem as ViewOrder;
                    orderId = selectedOrder?.OrderId;
                }
                if (orderId != null)
                {
                    using (CodeverseContext db = new CodeverseContext())
                    {
                        Order? order = db.Orders
                            .FirstOrDefault(p => p.OrderId == orderId);
                        if (order != null && _employee != null)
                        {
                            var orderEmployee = db.OrderEmployees
                                .FirstOrDefault(p => p.EmployeeId == _employee.EmployeeId
                                && p.OrderId == order.OrderId);

                            if (orderEmployee == null && _employee.PositionId == 3)
                            {
                                OrderEmployee oe = new OrderEmployee()
                                {
                                    OrderId = order.OrderId,
                                    EmployeeId = _employee.EmployeeId,
                                    Date = DateTime.Now,
                                };
                                db.OrderEmployees.Add(oe);
                            }
                            if (stateId == 7)
                            {
                                CompletedOrder co = new CompletedOrder()
                                {
                                    OrderId = order.OrderId,
                                    Date = DateOnly.FromDateTime(DateTime.Now),
                                };
                                db.CompletedOrders.Add(co);
                            }
                            order.StateId = stateId;
                            db.SaveChanges();
                            Refresh();
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void CheckLatestOrders()
        {
            using (CodeverseContext db = new CodeverseContext())
            {
                DateTime lastWeek = DateTime.Now.AddDays(-7);
                var orders = db.Orders
                    .Where(p => p.Date < DateOnly.FromDateTime(lastWeek)
                    && p.StateId == 1);
                foreach(var order in orders)
                {
                    MessageBox.Show($"Заказ №{order.OrderId} находится в режиме ожидания больше недели.");
                }
            }
        }

        private void BtnShowInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(_selectedOrder != null)
                {
                    WindowShowInfo windowShowInfo = new WindowShowInfo(_selectedOrder.OrderId);
                    windowShowInfo.ShowDialog();
                    Refresh();
                }
                else
                {
                    var acceptorder = ListAcceptOrder.SelectedItem as ViewOrderEmployee;
                    var completedorder = ListCompleteOrder.SelectedItem as ViewOrderEmployee;
                    if (acceptorder != null)
                    {                        
                        WindowShowInfo windowShowInfo = new WindowShowInfo(acceptorder.OrderId);
                        windowShowInfo.ShowDialog();
                        Refresh();
                    }
                    else if (completedorder != null)
                    {
                        WindowShowInfo windowShowInfo = new WindowShowInfo(completedorder.OrderId);
                        windowShowInfo.ShowDialog();
                        Refresh();
                    }
                    else
                    {
                        var neworder = ListNewOrder.SelectedItem as ViewOrder;
                        WindowShowInfo windowShowInfo = new WindowShowInfo(neworder.OrderId);
                        windowShowInfo.ShowDialog();
                        Refresh();
                    }
                }
            }
            catch(Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}
