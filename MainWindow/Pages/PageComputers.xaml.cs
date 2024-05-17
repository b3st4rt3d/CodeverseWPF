using CodeverseWPF.Utils;
using CodeverseWPF.Windows;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CodeverseWPF.MainWindow.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageComputers.xaml
    /// </summary>
    public partial class PageComputers : Page
    {
        Device? _selectedDevice;
        byte[]? _imageData;
        public PageComputers()
        {
            InitializeComponent();
            Refresh();
            SPChange.IsEnabled = false;
            BtnDelete.IsEnabled = false;
            BtnChange.IsEnabled = false;
        }

        private void Refresh()
        {
            try
            {
                using (CodeverseContext  db = new CodeverseContext())
                {
                    var devices = db.Devices;
                    ListDevices.ItemsSource = devices.ToArray();
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
                    var devices = db.Devices
                        .Where(p => p.Device1.ToLower().Contains(searchString));
                    ListDevices.ItemsSource = devices.ToArray();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
            TBSearch.Clear();
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (CodeverseContext db = new CodeverseContext())
                {
                    var devices = db.Devices;
                    int lastindex = devices.Count();

                    var wcd = new WindowCreateDevice();
                    wcd.ShowDialog();

                    var device = db.Devices
                        .FirstOrDefault(p => p.DeviceId == lastindex + 1);
                    if (device != null) ResentAct.Text = $"Создан компьютер {device.Device1}.";
                    Refresh();
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnChange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedDevice != null)
                {
                    using (CodeverseContext db = new CodeverseContext())
                    {
                        var device = db.Devices
                            .FirstOrDefault(p => p.DeviceId == _selectedDevice.DeviceId);
                        var name = TBDevice.Text.Trim();
                        var price = Convert.ToDecimal(TBPrice.Text.Trim());
                        if (device != null 
                            && !string.IsNullOrEmpty(name) 
                            && price > 0)
                        {
                            device.Device1 = name;
                            device.Price = price;
                            device.Image = _imageData;
                            db.SaveChanges();
                            Refresh();
                            ResentAct.Text = $"Изменен компьютер {name}.";
                            SPChangeBtns.Visibility = Visibility.Visible;
                            SPMainBtns.Visibility = Visibility.Collapsed;
                            ListDevices.IsEnabled = false;
                        }
                    }
                }
            }
            catch(Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Если вы удалите компьютер, его нельзя будет восстановить. Удалить компьютер?",
                    "Удаление компьютер",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (_selectedDevice != null)
                    {
                        using (CodeverseContext db = new CodeverseContext())
                        {
                            var device = db.Devices
                                .FirstOrDefault(p => p.DeviceId == _selectedDevice.DeviceId);
                            var config = db.Configs
                                .Where(p => p.DeviceId == _selectedDevice.DeviceId);

                            if(device != null)
                            {
                                foreach (var i in config)
                                {
                                    db.Configs.Remove(i);
                                }
                                db.Devices.Remove(device);

                                db.SaveChanges();
                                ResentAct.Text = $"Удалена деталь {_selectedDevice.Device1} {_selectedDevice.Price} Руб.";
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
            SPChangeBtns.Visibility = Visibility.Collapsed;
            SPMainBtns.Visibility = Visibility.Visible;
            ListDevices.IsEnabled = true;
            ClearTB();
        }

        private void BtnReturnChange_Click(object sender, RoutedEventArgs e)
        {
            SPChangeBtns.Visibility = Visibility.Collapsed;
            SPMainBtns.Visibility = Visibility.Visible;
            ListDevices.IsEnabled = true;
            ClearTB();
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

        private void ListDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedDevice = ListDevices.SelectedItem as Device;
            if (_selectedDevice != null)
            {
                SPChange.IsEnabled = true;
                BtnDelete.IsEnabled = true;
                BtnChange.IsEnabled = true;
                TBDevice.Text = _selectedDevice.Device1.Trim();
                TBPrice.Text = _selectedDevice.Price.ToString();
                byte[]? imageData = _selectedDevice.Image;                
                if (imageData != null) Photo.Source = ImageTools.ByteToImage(imageData);
                using (CodeverseContext db = new CodeverseContext())
                {
                    var config = db.ViewConfigs
                        .Where(p => p.DeviceId == _selectedDevice.DeviceId);
                    ListConfig.ItemsSource = config.ToArray();
                }
            }
            else
            {
                SPChange.IsEnabled = false;
                BtnDelete.IsEnabled = false;
                BtnChange.IsEnabled = false;
            }
        }

        private void ClearTB()
        {
            TBDevice.Clear();
            TBPrice.Clear();
            ListConfig.Items.Clear();
        }

        private void BtnChangeConfig_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedDevice != null)
            {
                var wcc = new WindowChangeConfig(_selectedDevice.DeviceId);
                wcc.ShowDialog();
                Refresh();
            }
        }
    }
}
