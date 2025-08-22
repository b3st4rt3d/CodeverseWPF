using System.Windows;
using CodeverseWPF.DB;
using Type = CodeverseWPF.DB.Type;

namespace CodeverseWPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowOrderDetails.xaml
    /// </summary>
    public partial class WindowOrderDetails : Window
    {
        int? _orderId;
        public WindowOrderDetails(int orderId)
        {
            InitializeComponent();
            _orderId = orderId;
            Refresh();
        }

        private void Refresh()
        {
            try
            {
                using(CodeverseContext db = new CodeverseContext())
                {
                    var details = db.ViewDetails;
                    ListDetails.ItemsSource = details.ToList();

                    if(_orderId != null)
                    {
                        var orderDetails = db.ViewOrderDetails
                            .Where(p => p.OrderId == _orderId);
                        ListOrderDetails.ItemsSource = orderDetails.ToList();
                    }

                    var brands = db.Brands
                        .OrderBy(p => p.BrandId);
                    CBBrandFilter.ItemsSource = brands.ToArray();

                    var types = db.Types
                        .OrderBy(p => p.TypeId); ;
                    CBTypeFilter.ItemsSource = types.ToArray();
                }
            }
            catch(Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var searchString = TBSearch.Text.Trim().ToLower();
                using (CodeverseContext db = new CodeverseContext())
                {
                    var details = db.ViewDetails
                        .Where(p => p.Detail.ToLower().Contains(searchString)
                            || p.Description.ToLower().Contains(searchString));

                    ListDetails.ItemsSource = details.ToArray();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void CBTypeFilter_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                var type = CBTypeFilter.SelectedItem as Type;
                if (type != null)
                {
                    using (CodeverseContext db = new CodeverseContext())
                    {
                        var details = db.ViewDetails
                            .Where(p => p.TypeId == type.TypeId);
                        ListDetails.ItemsSource = details.ToArray();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void CBBrandFilter_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                var brand = CBBrandFilter.SelectedItem as Brand;
                if (brand != null)
                {
                    using (CodeverseContext db = new CodeverseContext())
                    {
                        var details = db.ViewDetails
                            .Where(p => p.BrandId == brand.BrandId);
                        ListDetails.ItemsSource = details.ToArray();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void ListDetails_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                var selectedDetail = ListDetails.SelectedItem as ViewDetail;
                if (selectedDetail != null && _orderId != null)
                {
                    using (CodeverseContext db = new CodeverseContext())
                    {
                        var detail = db.Details
                            .FirstOrDefault(p => p.DetailId == selectedDetail.DetailId);
                        if (detail != null)
                        {
                            if (detail.Count > 0) detail.Count--;
                            else
                            {
                                string sMessageBoxText = "Недостатосное количетсво деталей";
                                string sCaption = "Хотите ли вы запросить недостаточное количество деталей?";

                                MessageBoxButton btnMessageBox = MessageBoxButton.YesNo;
                                MessageBoxImage icnMessageBox = MessageBoxImage.Warning;

                                MessageBoxResult rsltMessageBox = MessageBox.Show(sMessageBoxText, sCaption, btnMessageBox, icnMessageBox);

                                if (rsltMessageBox == MessageBoxResult.Yes) MakeRequestDetails(detail, 1);
                                return;
                            }
                        }

                        var unique = db.OrderDetails
                            .FirstOrDefault(p => p.OrderId == _orderId
                            && p.DetailId == selectedDetail.DetailId);
                        if(unique != null) unique.Count++;
                        else
                        {
                            OrderDetail orderDetail = new OrderDetail { 
                                OrderId = (int)_orderId,
                                DetailId = selectedDetail.DetailId,
                                Count = 1,
                            };
                            db.OrderDetails.Add(orderDetail);
                        }

                        var order = db.Orders
                            .FirstOrDefault(p => p.OrderId == _orderId);
                        if (order != null) order.Total += selectedDetail.Price;

                        db.SaveChanges();
                        Refresh();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void ListOrderDetails_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                var selectedDetail = ListOrderDetails.SelectedItem as ViewOrderDetail;
                if (selectedDetail != null && _orderId != null)
                {
                    using (CodeverseContext db = new CodeverseContext())
                    {
                        var orderDetail = db.OrderDetails
                            .FirstOrDefault(p => p.OrderId == _orderId
                            && p.DetailId == selectedDetail.DetailId);

                        if (orderDetail != null)
                        {
                            if (orderDetail.Count != 1) orderDetail.Count--;
                            else db.OrderDetails.Remove(orderDetail);
                        }

                        var order = db.Orders
                            .FirstOrDefault(p => p.OrderId == _orderId);
                        if (order != null) order.Total -= selectedDetail.Price;

                        var detail = db.Details
                            .FirstOrDefault(p => p.DetailId == selectedDetail.DetailId);
                        if (detail != null) detail.Count++;

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
