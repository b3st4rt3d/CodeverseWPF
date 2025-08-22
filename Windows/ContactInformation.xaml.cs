using System.Windows;
using CodeverseWPF.DB;

namespace CodeverseWPF.Windows
{
    /// <summary>
    /// Логика взаимодействия для ContactInformation.xaml
    /// </summary>
    public partial class ContactInformation : Window
    {
        public ContactInformation(int orderID)
        {
            InitializeComponent();
            try
            {
                using (CodeverseContext db = new CodeverseContext())
                {
                    var order = db.Orders
                        .FirstOrDefault(o => o.OrderId == orderID);

                    if (order != null)
                    {
                        var client = db.Clients
                            .FirstOrDefault(o => o.ClientId == order.ClientId);

                        if (order != null && client != null)
                        {
                            TBOrder.Text = $"#{orderID}";
                            TBMail.Text = client.Email;
                            TBName.Text = client.Name + " " + client.Surname;
                            TBPhone.Text = client.Phone;
                            TBPrice.Text = order.Total.ToString() + " Руб.";
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("Неудалось найти полную информацию о заказе. Были выставлены значения по умолчанию.");
            }
        }
    }
}
