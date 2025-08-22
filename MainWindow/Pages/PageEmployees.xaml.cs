using CodeverseWPF.DB;
using CodeverseWPF.Utils;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace CodeverseWPF.MainWindow.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageEmployees.xaml
    /// </summary>
    public partial class PageEmployees : Page
    {
        byte[]? _imageData;
        ViewEmployee? _selectedEmployee;

        public PageEmployees()
        {
            InitializeComponent();
            Refresh();
            SPChangeBtns.Visibility = Visibility.Collapsed;
            BtnChange.IsEnabled = false;
            BtnDelete.IsEnabled = false;
        }

        private void Refresh()
        {
            try
            {
                using (CodeverseContext db = new CodeverseContext())
                {
                    var employees = db.ViewEmployees;
                    ListEmployees.ItemsSource = employees.ToList();

                    var positions = db.Positions
                        .OrderBy(p => p.PositionId);
                    CBTPosition.ItemsSource = positions.ToList();
                    CBTPositionFilter.ItemsSource = positions.ToList();
                }
            }
            catch(Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
            TBSearch.Clear();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var searchString = TBSearch.Text.Trim().ToLower();
                using (CodeverseContext db = new CodeverseContext())
                {
                    var employees = db.ViewEmployees
                        .Where(p => p.Name.ToLower().Contains(searchString)
                        || p.Surname.ToLower().Contains(searchString)
                        || p.Email.ToLower().Contains(searchString)
                        || p.Password.ToLower().Contains(searchString)
                        || p.Login.ToLower().Contains(searchString));
                    ListEmployees.ItemsSource = employees.ToList();
                    ResentAct.Text = $"Найдено {employees.Count()} записей";
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void CBTPositionFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var position = CBTPositionFilter.SelectedItem as Position;
                using (CodeverseContext db = new CodeverseContext())
                {
                    if (position != null)
                    {
                        var employees = db.ViewEmployees
                            .Where(p => p.PositionId == position.PositionId);
                        ListEmployees.ItemsSource = employees.ToList();
                        ResentAct.Text = $"Найдено {employees.Count()} записей";
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var name = TBName.Text.Trim();
                var surname = TBSurname.Text.Trim();
                var email = TBEmail.Text.Trim();
                var login = TBLogin.Text.Trim();
                var password = Hashing.Encrypt(TBPassword.Text.Trim());
                var position = CBTPosition.SelectedItem as Position;
                if (position == null
                    || string.IsNullOrEmpty(name)
                    || string.IsNullOrEmpty(surname)
                    || string.IsNullOrEmpty(email)
                    || string.IsNullOrEmpty(login)
                    || string.IsNullOrEmpty(password)) MessageBox.Show("Заполните все поля");
                else
                {
                    if (!Validate.IsValidEmail(email)) return;

                    Employee employee = new Employee
                    {
                        Name = name,
                        Surname = surname,
                        Email = email,
                        Login = login,
                        Password = password,
                        PositionId = position.PositionId
                    };
                    if (MessageBox.Show("Подтвердите создание нового сотрудника.",
                    "Создание сотрудника",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        using(CodeverseContext db = new CodeverseContext())
                        {
                            db.Employees.Add(employee);
                            db.SaveChanges();
                            Refresh();
                            ResentAct.Text = $"Создан новый сотрудник {surname} {name}";
                        }
                    }
                }
            }
            catch(Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnChange_Click(object sender, RoutedEventArgs e)
        {
            ListEmployees.IsEnabled = false;
            SPChangeBtns.Visibility = Visibility.Visible;
            SPFilterBtns.IsEnabled = false;
            SPMainBtns.Visibility = Visibility.Collapsed;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Если вы удалите сотрудника, его нельзя будет восстановить. Удалить сотрудника?",
                    "Удаление сотрудника",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (_selectedEmployee != null)
                    {
                        using (CodeverseContext db = new CodeverseContext())
                        {
                            if (_selectedEmployee.PositionId == 1)
                            {
                                MessageBox.Show("Редактирование директора запрещено.");
                                return;
                            }
                            var employee = db.Employees
                                .FirstOrDefault(p => p.EmployeeId == _selectedEmployee.EmployeeId);
                            if(employee != null)
                            {
                                var orderEmployees = db.OrderEmployees
                                    .Where(p => p.EmployeeId == employee.EmployeeId);
                                foreach(var orderEmployee in orderEmployees) db.OrderEmployees.Remove(orderEmployee);

                                db.Employees.Remove(employee);
                                db.SaveChanges();
                                Refresh();
                                ResentAct.Text = $"Удален сотрудник {employee.Surname} {employee.Name}";
                            }
                        }
                    }
                    else MessageBox.Show("Выберите сотрудника.");
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnCompleteChange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedEmployee != null)
                {
                    var name = TBName.Text.Trim();
                    var surname = TBSurname.Text.Trim();
                    var email = TBEmail.Text.Trim();
                    var login = TBLogin.Text.Trim();
                    var password = Hashing.Encrypt(TBPassword.Text.Trim());
                    var position = CBTPosition.SelectedItem as Position;
                    if (position == null
                        || string.IsNullOrEmpty(name)
                        || string.IsNullOrEmpty(surname)
                        || string.IsNullOrEmpty(email)
                        || string.IsNullOrEmpty(login)
                        || string.IsNullOrEmpty(password)) MessageBox.Show("Заполните все поля");
                    else
                    {
                        if (!Validate.IsValidEmail(email)) return;

                        using (CodeverseContext db = new CodeverseContext())
                        {
                            var employee = db.Employees
                                .FirstOrDefault(p => p.EmployeeId == _selectedEmployee.EmployeeId);
                            if(employee != null)
                            {
                                employee.Name = name;
                                employee.Surname = surname;
                                employee.Email = email;
                                employee.Login = login;
                                employee.Password = password;
                                employee.PositionId = position.PositionId;
                                employee.Image = _imageData;

                                db.SaveChanges();
                                Refresh();
                                ResentAct.Text = $"Обновлен сотрудник {surname} {name}";
                            }
                            ListEmployees.IsEnabled = true;
                            SPChangeBtns.Visibility = Visibility.Collapsed;
                            SPFilterBtns.IsEnabled = true;
                            SPMainBtns.Visibility = Visibility.Visible;
                            ClearTB();
                        }
                    }
                }
                else MessageBox.Show("Выберите сотрудника");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnReturnChange_Click(object sender, RoutedEventArgs e)
        {
            ListEmployees.IsEnabled = true;
            SPChangeBtns.Visibility = Visibility.Collapsed;
            SPFilterBtns.IsEnabled = true;
            SPMainBtns.Visibility = Visibility.Visible;
            ClearTB();
        }

        private void ClearTB()
        {
            TBEmail.Clear();
            TBLogin.Clear();
            TBPassword.Clear();
            TBName.Clear();
            TBSurname.Clear();
            CBTPosition.SelectedIndex = -1;
        }

        private void ListEmployees_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedEmployee = ListEmployees.SelectedItem as ViewEmployee;
            if (_selectedEmployee != null)
            {
                TBName.Text = _selectedEmployee.Name;
                TBSurname.Text = _selectedEmployee.Surname;
                TBEmail.Text = _selectedEmployee.Email;
                TBPassword.Text = Hashing.Decrypt(_selectedEmployee.Password);
                TBLogin.Text = _selectedEmployee.Login;
                CBTPosition.SelectedIndex = _selectedEmployee.PositionId - 1;

                if (_selectedEmployee.Image != null)
                {
                    _imageData = _selectedEmployee.Image;
                    Photo.Source = ImageTools.ByteToImage(_selectedEmployee.Image);
                }
                else
                {
                    _imageData = null;
                    Photo.Source = null;
                }

                BtnChange.IsEnabled = true;
                BtnDelete.IsEnabled = true;
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
    }
}
