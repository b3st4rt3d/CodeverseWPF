using CodeverseWPF.DB;
using CodeverseWPF.MainWindow.Pages;
using CodeverseWPF.Utils;
using System.Windows;
using System.Windows.Threading;

namespace CodeverseWPF.MainWindow
{
    /// <summary>
    /// Логика взаимодействия для Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        Employee? _employee;
        public Main(Employee? employee)
        {
            InitializeComponent();
            MainFrame.Content = new PageClient();
            _employee = employee;
            BtnAllOrders.Visibility = Visibility.Collapsed;
            BtnOrders.Visibility = Visibility.Collapsed;
            BtnDevice.Visibility = Visibility.Collapsed;
            BtnDetail.Visibility = Visibility.Collapsed;
            BtnReports.Visibility = Visibility.Collapsed;
            BtnClients.Visibility = Visibility.Collapsed;
            BtnEmployee.Visibility = Visibility.Collapsed;
            BtnImportExport.Visibility = Visibility.Collapsed;
            BtnRequest.Visibility = Visibility.Collapsed;
            int position = (int)employee.PositionId;
            if (position == 1)
            {
                BtnAllOrders.Visibility = Visibility.Visible;
                BtnReports.Visibility = Visibility.Visible;
                BtnEmployee.Visibility = Visibility.Visible;
            }
            else if(position == 2)
            {
                BtnAllOrders.Visibility = Visibility.Visible;
                BtnOrders.Visibility = Visibility.Visible;
                BtnDevice.Visibility = Visibility.Visible;
                BtnDetail.Visibility = Visibility.Visible;
                BtnReports.Visibility = Visibility.Visible;
                BtnClients.Visibility = Visibility.Visible;
                BtnEmployee.Visibility = Visibility.Visible;
                BtnImportExport.Visibility = Visibility.Visible;
                BtnRequest.Visibility = Visibility.Visible;
            }
            else if (position == 3)
            {
                BtnAllOrders.Visibility = Visibility.Visible;
                BtnDevice.Visibility = Visibility.Visible;
                BtnDetail.Visibility = Visibility.Visible;
            }
            else if (position == 4)
            {
                BtnClients.Visibility = Visibility.Visible;
                BtnAllOrders.Visibility = Visibility.Visible;
                BtnOrders.Visibility = Visibility.Visible;
            }
            else
            {
                BtnRequest.Visibility = Visibility.Visible;
                BtnDetail.Visibility= Visibility.Visible;
                BtnDevice.Visibility = Visibility.Visible;
            }

            LoadUser();
            SetupTimer();
        }

        public void LoadUser()
        {
            if (_employee != null)
            {
                using (CodeverseContext db = new CodeverseContext())
                {
                    var user = db.ViewEmployees
                        .FirstOrDefault(p => p.EmployeeId == _employee.EmployeeId);
                    if (user != null)
                    {
                        TBUserName.Text = $"{user.Surname} {user.Name}";
                        TBUserPosition.Text = $"{user.Position}";
                        if (user.Image != null)
                        {
                            UserPhoto.Source = ImageTools.ByteToImage(user.Image);
                        }
                    }
                }
            }
        }

        private void SetupTimer()
        {
            DispatcherTimer _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(5);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            LoadUser();
        }

        private void BtnDetail_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new PageDetail();
        }

        private void BtnClients_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new PageClient();
        }

        private void BtnDevice_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new PageComputers();
        }

        private void BtnOrders_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new PageOrder();
        }

        private void BtnEmployee_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new PageEmployees();
        }

        private void BtnAllOrders_Click(object sender, RoutedEventArgs e)
        {
            if(_employee != null)
            {
                MainFrame.Content = new PageAcceptOrder(_employee);
            }
        }

        private void BtnReports_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new PageReports();
        }

        private void BtnImportExport_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new PageImportExport();
        }

        private void BtnProfile_Click(object sender, RoutedEventArgs e)
        {
            if (_employee != null)
            {
                MainFrame.Content = new PageProfile(_employee.EmployeeId);
            }
        }

        private void BtnAbout_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new PageAbout();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnRequest_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new PageReequest();
        }
    }
}
