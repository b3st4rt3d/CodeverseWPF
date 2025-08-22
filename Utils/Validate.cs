using System.Text.RegularExpressions;
using System.Windows;

namespace CodeverseWPF.Utils
{
    internal class Validate
    {
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            try
            {
                string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                bool isValid = Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
                if (isValid) return true;
                else
                {
                    MessageBox.Show($"Почта {email} не корректна. \n Пример почты - myemail@mail.ru");
                    return false;
                }
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        public static bool IsValidPhoneNumber(string number)
        {
            if (string.IsNullOrWhiteSpace(number))
            {
                return false;
            }

            try
            {
                string pattern = @"^(\+?\d{1,4}[\s-]?)?((\(\d{1,4}\))|\d{1,4})[\s-]?\d{1,4}[\s-]?\d{1,4}[\s-]?\d{1,9}$";
                bool isValid = Regex.IsMatch(number, pattern, RegexOptions.IgnoreCase);
                if (isValid) return true;
                else
                {
                    MessageBox.Show($"Телофон {number} не корректен. \n Пример телефона - +7-999-100-11-22");
                    return false;
                }
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

    }
}
