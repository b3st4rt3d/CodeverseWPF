using CodeverseWPF.Utils;
using CodeverseWPF.Windows;
using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CodeverseWPF.MainWindow.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageDetail.xaml
    /// </summary>
    public partial class PageDetail : Page
    {
        ViewDetail? _selectedDetail;
        byte[]? _imageData;
        public PageDetail()
        {
            InitializeComponent();
            Refresh();
            BtnChange.IsEnabled = false;
            BtnDelete.IsEnabled = false;
        }

        private void Refresh()
        {
            using (CodeverseContext db = new CodeverseContext())
            {
                var details = db.ViewDetails;
                ListDetail.ItemsSource = details.ToArray();

                var brands = db.Brands
                    .OrderBy(p => p.BrandId);
                CBBrand.ItemsSource = brands.ToArray();
                CBBrandFilter.ItemsSource = brands.ToArray();

                var types = db.Types
                    .OrderBy(p => p.TypeId); ;
                CBType.ItemsSource = types.ToArray();
                CBTypeFilter.ItemsSource = types.ToArray();
            }
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
                        ResentAct.Text = $"Найдено {details.Count()} записей.";
                    }
                }
            }
            catch(Exception ex) { MessageBox.Show(ex.Message); }
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
                        ResentAct.Text = $"Найдено {details.Count()} записей.";
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var serachString = TBSearch.Text;
                using (CodeverseContext db = new CodeverseContext())
                {
                    var details = db.ViewDetails
                        .Where(p => p.Detail.ToLower().Contains(serachString.ToLower())
                        || p.Description.ToLower().Contains(serachString.ToLower()));

                    ListDetail.ItemsSource = details.ToArray();
                    ResentAct.Text = $"Найдено {details.Count()} записей.";
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
            TBSearch.Clear();
            ClearTB();
        }

        private void BtnCreate_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                var detail = TBDetail.Text;
                var price = Convert.ToDecimal(TBPrice.Text);
                var description = TBDescription.Text;
                var type = CBType.SelectedItem as Type;
                var brand = CBBrand.SelectedItem as Brand;

                if (string.IsNullOrEmpty(detail)
                    || price < 0
                    || type == null
                    || brand == null) MessageBox.Show("Заполните все поля.");
                else
                {
                    Detail newDetail = new Detail
                    {
                        Detail1 = detail,
                        Price = price,
                        Description = description,
                        TypeId = type?.TypeId,
                        BrandId = brand?.BrandId,
                        Image = _imageData,
                    };

                    using (CodeverseContext db = new CodeverseContext())
                    {
                        db.Details.Add(newDetail);
                        db.SaveChanges();
                        ResentAct.Text = $"Создана деталь {detail} {type?.Type1} {brand?.Brand1}";
                        Refresh();
                        ClearTB();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnChange_Click(object sender, RoutedEventArgs e)
        {
            SPChangeBtn.Visibility = Visibility.Visible;
            SPMainBtn.Visibility = Visibility.Collapsed;
            ListDetail.IsEnabled = false;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Если вы удалите деталь, его нельзя будет восстановить. Удалить деталь?",
                    "Удаление детали",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (_selectedDetail != null)
                    {
                        using (CodeverseContext db = new CodeverseContext())
                        {
                            var detail = db.Details
                                .FirstOrDefault(p => p.DetailId == _selectedDetail.DetailId);
                            if (detail != null)
                            {
                                db.Details.Remove(detail);
                                db.SaveChanges();
                                ResentAct.Text = $"Удалена деталь {_selectedDetail.Brand} {_selectedDetail.Detail} {_selectedDetail.Type}";
                                Refresh();
                            }
                            else MessageBox.Show("Выберите деталь");
                        }
                    }
                    else { MessageBox.Show("Выберите деталь"); }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnSaveChange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var name = TBDetail.Text;
                var price = Convert.ToDecimal(TBPrice.Text);
                var description = TBDescription.Text;
                var type = CBType.SelectedItem as Type;
                var brand = CBBrand.SelectedItem as Brand;

                if (string.IsNullOrEmpty(name)
                    || price < 0
                    || type == null
                    || brand == null
                    || _selectedDetail == null) MessageBox.Show("Заполните все поля.");
                else
                {
                    using (CodeverseContext db = new CodeverseContext())
                    {
                        var detail = db.Details
                            .FirstOrDefault(p => p.DetailId == _selectedDetail.DetailId);
                        if (detail != null)
                        {
                            detail.Detail1 = name;
                            detail.Price = price;
                            detail.Description = description;
                            detail.TypeId = type?.TypeId;
                            detail.BrandId = brand?.BrandId;
                            detail.Image = _imageData;

                            db.SaveChanges();
                            ResentAct.Text = $"Изменена деталь {detail} {type?.Type1} {brand?.Brand1}";
                            SPChangeBtn.Visibility = Visibility.Collapsed;
                            SPMainBtn.Visibility = Visibility.Visible;
                            ListDetail.IsEnabled = true;
                            Refresh();
                            ClearTB();
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnReturnChange_Click(object sender, RoutedEventArgs e)
        {
            SPChangeBtn.Visibility = Visibility.Collapsed;
            SPMainBtn.Visibility = Visibility.Visible;
            ListDetail.IsEnabled = true;
            ClearTB();
        }

        private void ListDetail_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BtnChange.IsEnabled = true;
            BtnDelete.IsEnabled = true;
            _selectedDetail = ListDetail.SelectedItem as ViewDetail;
            if (_selectedDetail != null)
            {
                TBDetail.Text = _selectedDetail?.Detail;
                TBPrice.Text = ((decimal)_selectedDetail.Price).ToString();
                CBBrand.SelectedIndex = _selectedDetail.BrandId - 1;
                CBType.SelectedIndex = _selectedDetail.TypeId - 1;
                TBDescription.Text = _selectedDetail?.Description?.ToString();
                byte[]? image = _selectedDetail?.Image;
                if (image != null)
                {
                    Photo.Source = ImageTools.ByteToImage(image);
                    _imageData = image;
                }
                else
                {
                    Photo.Source = null;
                    _imageData = null;
                }
            }

        }

        private void BtnSelectImg_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                string filename = op.FileName;
                byte[] imageData;
                using (System.IO.FileStream fs = new System.IO.FileStream(filename, FileMode.Open))
                {
                    imageData = new byte[fs.Length];
                    fs.Read(imageData, 0, imageData.Length);
                    _imageData = imageData;
                    Photo.Source = ImageTools.ByteToImage(imageData);
                }
            }
        }

        private void ClearTB()
        {
            TBDescription.Clear();
            TBDetail.Clear();
            TBPrice.Clear();
            CBBrand.SelectedIndex = -1;
            CBType.SelectedIndex = -1;
            Photo.Source = null;
            _imageData = null;
        }

        private void BtnCreateTypes_Click(object sender, RoutedEventArgs e)
        {
            WindowTypes windowTypes = new WindowTypes();
            windowTypes.ShowDialog();
        }

        private void BtnCreateBrands_Click(object sender, RoutedEventArgs e)
        {
            WindowBrands windowBrands = new WindowBrands();
            windowBrands.ShowDialog();
        }

        private void CBSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sortLabel = CBSort.SelectedItem as System.Windows.Controls.Label;
            if (sortLabel != null)
            {
                string sort = sortLabel.Content.ToString();
                if (sort == "По умолчанию") Refresh();
                else if (sort == "По имени")
                {
                    using(CodeverseContext db = new CodeverseContext())
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
    }
}
