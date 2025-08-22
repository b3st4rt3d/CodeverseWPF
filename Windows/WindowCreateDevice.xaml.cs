using CodeverseWPF.DB;
using CodeverseWPF.Utils;
using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace CodeverseWPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowCreateDevice.xaml
    /// </summary>
    public partial class WindowCreateDevice : Window
    {
        private byte[]? _imageData;
        public WindowCreateDevice()
        {
            InitializeComponent();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var name = TBName.Text.Trim();
                var price = Convert.ToDecimal(TBPrice.Text.Trim());
                if (price > 0
                    || !string.IsNullOrEmpty(name))
                {
                    Device device = new Device
                    {
                        Device1 = name,
                        Price = price,
                        Image = _imageData
                    };
                    using (CodeverseContext db = new CodeverseContext())
                    {
                        db.Devices.Add(device);
                        db.SaveChanges();
                        Close();
                    }
                }
                else { MessageBox.Show("Заполните все поля."); }
            }
            catch(Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void Return_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
