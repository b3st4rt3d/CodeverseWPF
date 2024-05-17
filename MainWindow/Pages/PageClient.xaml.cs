using System.Windows;
using System.Windows.Controls;

namespace CodeverseWPF.MainWindow.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageClient.xaml
    /// </summary>
    public partial class PageClient : Page
    {
        Client? _selectedClient;
        public PageClient()
        {
            InitializeComponent();
            SPChangeBtns.Visibility = Visibility.Collapsed;
            Refresh();
        }

        private void Refresh()
        {
            try
            {
                using (CodeverseContext db = new CodeverseContext())
                {
                    var clients = db.Clients;
                    ListClient.ItemsSource = clients.ToArray();
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
                    var clients = db.Clients
                        .Where(p => p.Name.ToLower().Contains(searchString)
                        || p.Surname.ToLower().Contains(searchString)
                        || p.Email.ToLower().Contains(searchString)
                        || p.Phone.ToLower().Contains(searchString));
                    ListClient.ItemsSource = clients.ToArray();
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
                var phone = TBPhone.Text.Trim();
                var login = TBLogin.Text.Trim();
                var password = TBPassword.Text.Trim();

                if (string.IsNullOrEmpty(name)
                    || string.IsNullOrEmpty(surname)
                    || string.IsNullOrEmpty(email)
                    || string.IsNullOrEmpty(phone)) MessageBox.Show("Заполните все поля");
                else
                {
                    Client client = new Client { 
                        Name = name,
                        Surname = surname,
                        Email = email,
                        Phone = phone,
                        Login = login,
                        Password = password
                    };
                    using (CodeverseContext db = new CodeverseContext())
                    {
                        db.Clients.Add(client);
                        db.SaveChanges();
                        ClearTB();
                        Refresh();
                        ResentAct.Text = $"Добавлен клиент {surname} {name}";
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnChange_Click(object sender, RoutedEventArgs e)
        {
            ListClient.IsEnabled = false;
            SPChangeBtns.Visibility = Visibility.Visible;
            SPMainBtns.Visibility = Visibility.Collapsed;
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Если вы удалите клиента, его нельзя будет восстановить. Удалить клиента?",
                    "Удаление клиента",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (_selectedClient != null)
                    {
                        using (CodeverseContext db = new CodeverseContext())
                        {
                            var client = db.Clients
                                .FirstOrDefault(p => p.ClientId == _selectedClient.ClientId);
                            if(client != null)
                            {
                                db.Clients.Remove(client);
                                db.SaveChanges();
                                ResentAct.Text = $"Удален клиент {_selectedClient.Surname} {_selectedClient.Name}";
                                Refresh();
                            }
                        }
                    }
                    else { MessageBox.Show("Выберите деталь"); }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnCompleteChange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var name = TBName.Text.Trim();
                var surname = TBSurname.Text.Trim();
                var email = TBEmail.Text.Trim();
                var phone = TBPhone.Text.Trim();
                var login = TBLogin.Text.Trim();
                var password = TBPassword.Text.Trim();

                if (string.IsNullOrEmpty(name)
                    || string.IsNullOrEmpty(surname)
                    || string.IsNullOrEmpty(email)
                    || string.IsNullOrEmpty(phone)
                    || _selectedClient == null) MessageBox.Show("Заполните все поля");
                else
                {                    
                    using (CodeverseContext db = new CodeverseContext())
                    {
                        var client = db.Clients
                            .FirstOrDefault(client => client.ClientId == _selectedClient.ClientId);

                        if (client != null)
                        {
                            client.Name = name;
                            client.Surname = surname;
                            client.Email = email;
                            client.Phone = phone;
                            client.Login = login;
                            client.Password = password;

                            db.SaveChanges();
                            ClearTB();
                            Refresh();
                            ListClient.IsEnabled = true;
                            SPChangeBtns.Visibility = Visibility.Collapsed;
                            SPMainBtns.Visibility = Visibility.Visible;
                            ResentAct.Text = $"Изменен клиент {surname} {name}";
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnReturnChange_Click(object sender, RoutedEventArgs e)
        {
            ClearTB();
            ListClient.IsEnabled = true;
            SPChangeBtns.Visibility = Visibility.Collapsed;
            SPMainBtns.Visibility = Visibility.Visible;
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
            TBSearch.Clear();
        }

        private void ClearTB()
        {
            TBName.Clear();
            TBSurname.Clear();
            TBEmail.Clear();
            TBPhone.Clear();
            TBLogin.Clear();
            TBPassword.Clear();
        }

        private void ListClient_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedClient = ListClient.SelectedItem as Client;
            if (_selectedClient != null )
            {
                TBName.Text = _selectedClient.Name;
                TBSurname.Text = _selectedClient.Surname;
                TBEmail.Text = _selectedClient.Email;
                TBPhone.Text = _selectedClient.Phone;
                TBLogin.Text = _selectedClient.Login;
                TBPassword.Text = _selectedClient.Password;
            }
        }
    }
}
