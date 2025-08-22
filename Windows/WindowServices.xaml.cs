using System.Windows;
using CodeverseWPF.DB;

namespace CodeverseWPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowServices.xaml
    /// </summary>
    public partial class WindowServices : Window
    {
        public WindowServices()
        {
            InitializeComponent();
            RefreshList();
        }

        private void RefreshList()
        {
            try
            {
                using (CodeverseContext db = new CodeverseContext())
                {
                    var services = db.Services
                        .OrderBy(p => p.ServiceId);

                    ListService.ItemsSource = services.ToList();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            RefreshList();
            TBSearch.Clear();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(TBSearch.Text))
                {
                    using (CodeverseContext db = new CodeverseContext())
                    {
                        string search = TBSearch.Text;

                        var services = db.Services
                            .Where(p => p.Service1.ToLower().Contains(search.ToLower())
                            || p.Price.ToString().Contains(search.ToLower()));

                        ListService.ItemsSource = services.ToList();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(TBName.Text) || string.IsNullOrEmpty(TBPrice.Text))
                {
                    MessageBox.Show("Пожалуйста, заполните все поля.");
                    return;
                }
                else
                {
                    string name = TBName.Text;
                    decimal price = decimal.Parse(TBPrice.Text);

                    Service service = new Service
                    {
                        Service1 = name,
                        Price = price,
                    };

                    using (CodeverseContext db = new CodeverseContext())
                    {
                        db.Services.Add(service);
                        db.SaveChanges();
                        RefreshList();
                        TBName.Clear();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedService = ListService.SelectedItem as Service;
                if (selectedService != null)
                {
                    if (MessageBox.Show("Если вы удалите услугу, его нельзя будет восстановить. Удалить услугу?",
                    "Удаление услуги",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        using (CodeverseContext db = new CodeverseContext())
                        {
                            var orders = db.OrderServices
                                .FirstOrDefault(p => p.ServiceId == selectedService.ServiceId);
                            if (orders != null)
                            {
                                MessageBox.Show("Услуга используется в системе. Удаленине прервано.");
                                return;
                            }
                            var service = db.Services
                                .FirstOrDefault(p => p.ServiceId == selectedService.ServiceId);
                            if (service != null)
                            {
                                db.Services.Remove(service);
                                db.SaveChanges();
                                RefreshList();
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Выберите услугу");
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ListService.SelectedItem != null)
                {
                    if (!string.IsNullOrEmpty(TBName.Text) && !string.IsNullOrEmpty(TBPrice.Text))
                    {
                        var selectedService = ListService.SelectedItem as Service;
                        using (CodeverseContext db = new CodeverseContext())
                        {
                            var service = db.Services
                                .FirstOrDefault(p => p.ServiceId == selectedService.ServiceId);
                            if (service != null)
                            {
                                service.Service1 = TBName.Text;
                                service.Price = decimal.Parse(TBPrice.Text);
                                db.SaveChanges();
                                RefreshList();
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Пожалуйста, заполните все поля.");
                        return;
                    }
                    CompleteChange();
                }
                else
                {
                    MessageBox.Show("Выберите бренд");
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnReturn_Click(object sender, RoutedEventArgs e)
        {
            CompleteChange();
        }

        private void CompleteChange()
        {
            ListService.IsEnabled = true;
            BtnSave.Visibility = Visibility.Collapsed;
            BtnReturn.Visibility = Visibility.Collapsed;
            BtnDelete.Visibility = Visibility.Visible;
            BtnCreate.Visibility = Visibility.Visible;
            BtnChange.Visibility = Visibility.Visible;
            TBName.Text = string.Empty;
            TBPrice.Text = string.Empty;
        }

        private void BtnChange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedService = ListService.SelectedItem as Service;
                if (ListService.SelectedItem != null)
                {
                    ListService.IsEnabled = false;
                    BtnSave.Visibility = Visibility.Visible;
                    BtnReturn.Visibility = Visibility.Visible;
                    BtnDelete.Visibility = Visibility.Collapsed;
                    BtnCreate.Visibility = Visibility.Collapsed;
                    BtnChange.Visibility = Visibility.Collapsed;
                    TBName.Text = selectedService?.Service1;
                    TBPrice.Text = selectedService?.Price.ToString();
                }
                else
                {
                    MessageBox.Show("Выберите услугу");
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}