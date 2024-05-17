using System.Windows;

namespace CodeverseWPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowBrands.xaml
    /// </summary>
    public partial class WindowBrands : Window
    {
        public WindowBrands()
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
                    var brands = db.Brands
                        .OrderBy(p => p.BrandId);

                    ListBrands.ItemsSource = brands.ToList();
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

                        var brands = db.Brands
                            .Where(p => p.Brand1.ToLower().Contains(search.ToLower()));

                        ListBrands.ItemsSource = brands.ToList();
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(TBName.Text))
                {
                    MessageBox.Show("Пожалуйста, заполните все поля.");
                    return;
                }
                else
                {
                    string name = TBName.Text;
                    Brand brand = new Brand()
                    {
                        Brand1 = name,
                    };
                    using (CodeverseContext db = new CodeverseContext())
                    {
                        db.Brands.Add(brand);
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
                var selectedBrand = ListBrands.SelectedItem as Brand;
                if (selectedBrand != null)
                {
                    if (MessageBox.Show("Если вы удалите бренд, его нельзя будет восстановить. Удалить бренд?",
                    "Удаление бренда",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        using (CodeverseContext db = new CodeverseContext())
                        {
                            var brand = db.Brands
                                .FirstOrDefault(p => p.BrandId == selectedBrand.BrandId);
                            if (brand != null)
                            {
                                db.Brands.Remove(brand);
                                db.SaveChanges();
                                RefreshList();
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Выберите бренд");
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ListBrands.SelectedItem != null)
                {
                    if (!string.IsNullOrEmpty(TBName.Text))
                    {
                        var selectedBrand = ListBrands.SelectedItem as Brand;
                        using (CodeverseContext db = new CodeverseContext())
                        {
                            var brand = db.Brands
                                .FirstOrDefault(p => p.BrandId == selectedBrand.BrandId);
                            if (brand != null)
                            {
                                brand.Brand1 = TBName.Text;
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
            ListBrands.IsEnabled = true;
            BtnSave.Visibility = Visibility.Collapsed;
            BtnReturn.Visibility = Visibility.Collapsed;
            BtnDelete.Visibility = Visibility.Visible;
            BtnCreate.Visibility = Visibility.Visible;
            BtnChange.Visibility = Visibility.Visible;
            TBName.Text = string.Empty;
        }

        private void BtnChange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selectedBrand = ListBrands.SelectedItem as Brand;
                if (ListBrands.SelectedItem != null)
                {
                    ListBrands.IsEnabled = false;
                    BtnSave.Visibility = Visibility.Visible;
                    BtnReturn.Visibility = Visibility.Visible;
                    BtnDelete.Visibility = Visibility.Collapsed;
                    BtnCreate.Visibility = Visibility.Collapsed;
                    BtnChange.Visibility = Visibility.Collapsed;
                    TBName.Text = selectedBrand?.Brand1;
                }
                else
                {
                    MessageBox.Show("Выберите бренд");
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}