using System.Windows;
using CodeverseWPF.DB;
using Type = CodeverseWPF.DB.Type;

namespace CodeverseWPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowTypes.xaml
    /// </summary>
    public partial class WindowTypes : Window
    {
        public WindowTypes()
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
                    var types = db.Types
                        .OrderBy(p => p.TypeId);

                    ListTypes.ItemsSource = types.ToList();
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

                        var types = db.Types
                            .Where(p => p.Type1.ToLower().Contains(search.ToLower()));

                        ListTypes.ItemsSource = types.ToList();
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
                    Type type = new Type()
                    {
                        Type1 = name,
                    };
                    using (CodeverseContext db = new CodeverseContext())
                    {
                        db.Types.Add(type);
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
                var selectedType = ListTypes.SelectedItem as Type;
                if (selectedType != null)
                {
                    if (MessageBox.Show("Если вы удалите тип, его нельзя будет восстановить. Удалить тип?",
                    "Удаление типа",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        using (CodeverseContext db = new CodeverseContext())
                        {
                            var detail = db.Details
                                .FirstOrDefault(p => p.TypeId == selectedType.TypeId);
                            if (detail != null)
                            {
                                MessageBox.Show("Тип используется в системе. Удаленине прервано.");
                                return;
                            }

                            var type = db.Types
                                .FirstOrDefault(p => p.TypeId == selectedType.TypeId);
                            if (type != null)
                            {
                                db.Types.Remove(type);
                                db.SaveChanges();
                                RefreshList();
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Выберите тип");
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ListTypes.SelectedItem != null)
                {
                    if (!string.IsNullOrEmpty(TBName.Text))
                    {
                        var selectedType = ListTypes.SelectedItem as Type;
                        using (CodeverseContext db = new CodeverseContext())
                        {
                            var type = db.Types
                                .FirstOrDefault(p => p.TypeId == selectedType.TypeId);
                            if (type != null)
                            {
                                type.Type1 = TBName.Text;
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
                    MessageBox.Show("Выберите тип");
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
            ListTypes.IsEnabled = true;
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
                var selectedType = ListTypes.SelectedItem as Type;
                if (ListTypes.SelectedItem != null)
                {
                    ListTypes.IsEnabled = false;
                    BtnSave.Visibility = Visibility.Visible;
                    BtnReturn.Visibility = Visibility.Visible;
                    BtnDelete.Visibility = Visibility.Collapsed;
                    BtnCreate.Visibility = Visibility.Collapsed;
                    BtnChange.Visibility = Visibility.Collapsed;
                    TBName.Text = selectedType?.Type1;
                }
                else
                {
                    MessageBox.Show("Выберите тип");
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}
