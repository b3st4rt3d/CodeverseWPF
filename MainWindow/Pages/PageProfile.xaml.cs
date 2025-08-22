using CodeverseWPF.DB;
using CodeverseWPF.Utils;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace CodeverseWPF.MainWindow.Pages
{
    /// <summary>
    /// Логика взаимодействия для PageProfile.xaml
    /// </summary>
    public partial class PageProfile : Page
    {
        int? _userID;
        byte[]? _imageData;
        public PageProfile(int userID)
        {
            InitializeComponent();
            _userID = userID;
            GetUser();
        }

        private void GetUser()
        {
            try
            {
                using(CodeverseContext db = new CodeverseContext())
                {
                    if (_userID != null)
                    {
                        var user = db.ViewEmployees
                            .FirstOrDefault(p => p.EmployeeId == _userID);
                        var employee = db.Employees
                            .FirstOrDefault(p => p.EmployeeId == _userID);

                        if (user != null)
                        {
                            TBEmail.Text = user.Email;
                            TBLogin.Text = user.Login;
                            TBPassword.Text = Hashing.Decrypt(user.Password);
                            TBName.Text = user.Name;
                            TBSurname.Text = user.Surname;
                            TBPosition.Text = user.Position;

                            if(user.Image != null) 
                            {
                                Photo.Source = ImageTools.ByteToImage(user.Image);
                                _imageData = user.Image;
                            }
                        }
                        SPChangeBtns.IsEnabled = false;
                    }
                }
            }
            catch(Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnCompleteChange_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_userID != null)
                {
                    var name = TBName.Text.Trim();
                    var surname = TBSurname.Text.Trim();
                    var email = TBEmail.Text.Trim();
                    var login = TBLogin.Text.Trim();
                    var password = Hashing.Encrypt(TBPassword.Text.Trim());
                    if (string.IsNullOrEmpty(name)
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
                                .FirstOrDefault(p => p.EmployeeId == _userID);
                            if (employee != null)
                            {
                                employee.Name = name;
                                employee.Surname = surname;
                                employee.Email = email;
                                employee.Login = login;
                                employee.Password = password;
                                employee.Image = _imageData;

                                db.SaveChanges();
                                ResentAct.Text = $"Обновлен сотрудник {surname} {name}";
                            }
                            db.SaveChanges();
                            GetUser();
                        }
                    }
                }
                else MessageBox.Show("Произошла ошибка.");
            }
            catch(Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void BtnReturnChange_Click(object sender, RoutedEventArgs e)
        {
            GetUser();
            SPChangeBtns.IsEnabled = false;
        }


        private void BtnChangePhoto_Click(object sender, RoutedEventArgs e)
        {
            SPChangeBtns.IsEnabled = true;
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

        private void TBName_TextChanged(object sender, TextChangedEventArgs e)
        {
            SPChangeBtns.IsEnabled = true;
        }

        private void TBSurname_TextChanged(object sender, TextChangedEventArgs e)
        {
            SPChangeBtns.IsEnabled = true;
        }

        private void TBEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            SPChangeBtns.IsEnabled = true;
        }

        private void TBLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            SPChangeBtns.IsEnabled = true;
        }

        private void TBPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            SPChangeBtns.IsEnabled = true;
        }
    }
}
