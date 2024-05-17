using CodeverseWPF.MainWindow.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CodeverseWPF.MainWindow
{
    /// <summary>
    /// Логика взаимодействия для Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();
            MainFrame.Content = new PageClient();
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
    }
}
