using System.Windows;
using System.Windows.Controls;
using CodeverseWPF.DB;
using Type = CodeverseWPF.DB.Type;

namespace CodeverseWPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowChangeConfig.xaml
    /// </summary>
    public partial class WindowChangeConfig : Window
    {
        int? _deviceId;
        ViewDetail? _selectedDetail;
        ViewConfig? _selectedConfig;
        public WindowChangeConfig(int deviceId)
        {
            InitializeComponent();
            _deviceId = deviceId;
            Refresh();
        }

        private void Refresh()
        {
            try
            {
                using (CodeverseContext db = new CodeverseContext())
                {
                    var details = db.ViewDetails;
                    ListDetail.ItemsSource = details.ToArray();

                    var configs = db.ViewConfigs
                        .Where(p => p.DeviceId == _deviceId);
                    ListConfig.ItemsSource = configs.ToArray();

                    var brands = db.Brands;
                    CBBrandFilter.ItemsSource = brands.ToArray();

                    var types = db.Types;
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
                        .Where(p => p.Detail.ToLower().Contains(searchString));
                    ListDetail.ItemsSource = details.ToArray();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void CBTypeFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
                        ListDetail.ItemsSource = details.ToArray();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void CBBrandFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
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
                        ListDetail.ItemsSource = details.ToArray();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void CBSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var sortLabel = CBSort.SelectedItem as System.Windows.Controls.Label;
                if (sortLabel != null)
                {
                    string sort = sortLabel.Content.ToString();
                    if (sort == "По умолчанию") Refresh();
                    else if (sort == "По имени")
                    {
                        using (CodeverseContext db = new CodeverseContext())
                        {
                            var details = db.ViewDetails
                                .OrderBy(p => p.Detail);
                            ListDetail.ItemsSource = details.ToArray();
                        }
                    }
                    else if (sort == "По цене")
                    {
                        using (CodeverseContext db = new CodeverseContext())
                        {
                            var details = db.ViewDetails
                                .OrderBy(p => p.Price);
                            ListDetail.ItemsSource = details.ToArray();
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
            TBSearch.Clear();
            CBBrandFilter.SelectedIndex = -1;
            CBSort.SelectedIndex = -1;
            CBTypeFilter.SelectedIndex = -1;
        }

        private void ListDetail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedDetail = ListDetail.SelectedItem as ViewDetail;
            try
            {
                if (_deviceId != null
                    && _selectedDetail != null) 
                {
                    using (CodeverseContext db = new CodeverseContext())
                    {
                        var unique = db.Configs
                            .FirstOrDefault(p => p.DeviceId == _deviceId
                            && p.DetailId == _selectedDetail.DetailId);

                        if (unique != null)
                        {
                            unique.Count++;
                        }
                        else
                        {
                            Config config = new Config
                            {
                                DetailId = _selectedDetail.DetailId,
                                DeviceId = _deviceId,
                                Count = 1
                            };
                            db.Configs.Add(config);
                        }
                        db.SaveChanges();
                        Refresh();

                    }
                }
            }
            catch(Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void ListConfig_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedConfig = ListConfig.SelectedItem as ViewConfig;
            try
            {
                if (_deviceId != null && _selectedConfig != null)
                {
                    using (CodeverseContext db = new CodeverseContext())
                    {
                        var config = db.Configs
                            .FirstOrDefault(p => p.ConfigId == _selectedConfig.ConfigId);
                        if (config != null)
                        {
                            int count = config.Count;
                            if (count == 1) db.Configs.Remove(config);
                            else config.Count -= count;

                            db.SaveChanges();
                            Refresh();
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void CalculateDevice()
        {

        }
    }
}
