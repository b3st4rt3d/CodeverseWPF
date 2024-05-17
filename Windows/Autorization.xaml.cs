using CodeverseWPF.MainWindow;
using CodeverseWPF.Utils;
using System.Windows;
using System.Windows.Media;


namespace CodeverseWPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для Autorization.xaml
    /// </summary>
    public partial class Autorization : Window
    {
        public Autorization()
        {
            InitializeComponent();
        }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            var login = Login.Text.Trim();
            var password = Password.Password.Trim();

            if (string.IsNullOrEmpty(login))
            {
                Login.BorderBrush = new SolidColorBrush(Colors.Red);
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                Password.BorderBrush = new SolidColorBrush(Colors.Red);
                return;
            }

            string hashPassword = Hashing.HashingPassword(password);

            using (CodeverseContext db = new CodeverseContext())
            {
                var user = db.Employees.FirstOrDefault(
                    p => p.Login == login && p.Password == hashPassword);
                if (user != null)
                {
                    var mainWindow = new Main();
                    mainWindow.Show();
                    Close();
                }
                else
                {
                    MessageBox.Show("Неправильный логин или пароль.");
                }
            }            
        }        
    }
}
