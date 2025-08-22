using CodeverseWPF.DB;
using System.Windows;
using System.Windows.Controls;

namespace CodeverseWPF.MainWindow.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageReequest.xaml
    /// </summary>
    public partial class PageReequest : Page
    {
        ViewDetail? _selectedDetail;
        ViewDetailRequest? _selectedRequest;
        public PageReequest()
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
                    var details = db.ViewDetails;
                    var requests = db.ViewDetailRequests;
                    ListRequest.ItemsSource = requests.ToList();
                    ListDetail.ItemsSource = details.ToList();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void ListDetail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedDetail = ListDetail.SelectedItem as ViewDetail;
            SPDetailsBtn.IsEnabled = true;
            BtnCloseRequest.IsEnabled = false;
        }

        private void ListRequest_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedRequest = ListRequest.SelectedItem as ViewDetailRequest;
            SPDetailsBtn.IsEnabled = false;
            BtnCloseRequest.IsEnabled = true;
        }

        private void BtnAddDetail_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedDetail != null)
                {
                    bool isInt = int.TryParse(TBCount.Text, out int res);
                    if (isInt)
                    {
                        int count = int.Parse(TBCount.Text);
                        using (CodeverseContext db = new CodeverseContext())
                        {
                            var detail = db.Details
                                .FirstOrDefault(p => p.DetailId == _selectedDetail.DetailId);
                            if (detail != null)
                            {
                                var requests = db.Requests
                                    .Where(p => p.DetailId == _selectedDetail.DetailId);
                                foreach (var i in requests)
                                {
                                    if (i.Count <= count)
                                    {
                                        CloseRequest(i);
                                        count -= (int)i.Count;
                                    }
                                }
                                detail.Count += count;
                                db.SaveChanges();
                                Refresh();
                                TBCount.Clear();
                            }
                        }
                    }
                    else MessageBox.Show("Неправильный формат числа, необходимо ввести целое число.");
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnCloseRequest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedRequest != null)
                {
                    using (CodeverseContext db = new CodeverseContext())
                    {
                        var request = db.Requests
                            .FirstOrDefault(p => p.RequestId == _selectedRequest.RequestId);
                        if (request != null) CloseRequest(request);
                        Refresh();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void CloseRequest(Request request)
        {
            try
            {
                using (CodeverseContext db = new CodeverseContext())
                {
                    var req = db.Requests
                        .FirstOrDefault(p => p.RequestId == request.RequestId);
                    var order = db.Orders
                        .FirstOrDefault(p => p.OrderId == request.OrderId);
                    var detail = db.Details
                        .FirstOrDefault(p => p.DetailId == request.DetailId);
                    if (order != null && req != null && detail != null)
                    {
                        var orderDetail = new OrderDetail
                        {
                            DetailId = detail.DetailId,
                            OrderId = order.OrderId,
                            Count = request.Count,
                        };
                        var orderEmployee = db.OrderEmployees
                            .FirstOrDefault(p => p.OrderId == order.OrderId);
                        if (orderEmployee != null) order.StateId = 3;
                        else order.StateId = 1;

                        db.OrderDetails.Add(orderDetail);
                        db.Requests.Remove(req);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}
