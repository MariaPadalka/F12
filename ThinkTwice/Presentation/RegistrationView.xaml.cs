using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using BLL;

namespace Presentation
{
    public partial class RegistrationView : Page
    {
        RegistrationService reg_serv = new RegistrationService();
        public RegistrationView()
        {
            InitializeComponent();
        }
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            //Login login = new Login();
            //contentContainer.Content = login;
            //nav.Navigate(new Uri("page2.xaml", UriKind.RelativeOrAbsolute));
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new Uri("Login.xaml", UriKind.Relative));
        }
        private void GoToDashboard(object sender, RoutedEventArgs e)
        {
            //Dashboard dashboard = new Dashboard();
            //contentContainer.Content = dashboard;
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new Uri("Dashboard.xaml", UriKind.Relative));
        }
        public void Reset()
        {
            textBoxFirstName.Text = "";
            textBoxLastName.Text = "";
            textBoxEmail.Text = "";
            passwordBox1.Password = "";
            passwordBoxConfirm.Password = "";
        }
        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            string email = textBoxEmail.Text;
            string password = passwordBox1.Password;
            string first_name = textBoxFirstName.Text;
            string last_name = textBoxLastName.Text;
            DateTime? date = datePickerBirthdate.SelectedDate;
            string currency = comboBoxCurrency.Text;
            bool all_fields_present = email != "" && password != "" && first_name != "" && last_name != "" && date.HasValue && currency != "";;

            if (!all_fields_present || passwordError.Text != "" || confirmPassError.Text != "" || firstNameError.Text != "" || lastNameError.Text != "" || emailError.Text != "" || dateError.Text != "")
            {
                errormessage.Text = "Будь ласка, введіть коректні дані у всіх полях.";
            }
            else
            {
                var user = reg_serv.Register(email, password, first_name, last_name, date, currency);
                if (user != null)
                {
                    errormessage.Text = "Ви успішно зареєструвались.";
                    App.SetCurrentUser(user);
                    GoToDashboard(sender, e);
                }
                else
                {
                    errormessage.Text = "Ця пошта вже комусь належить.";
                }
            }
        }
        public static bool IsEmailValid(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            string emailPattern = @"^[a-zA-Z0-9._%+-]{3,20}@[a-zA-Z0-9.-]{2,20}\.[a-zA-Z]{2,10}$";

            return Regex.IsMatch(email, emailPattern);
        }
        public static bool IsPasswordValid(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,20}$";

            return Regex.IsMatch(password, passwordPattern);
        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = (PasswordBox)sender;
            string password = passwordBox.Password;

            if (IsPasswordValid(password))
            {
                passwordError.Text = ""; // Clear any previous error message
            }
            else if (password.Length < 8 || password.Length > 20)
            {
                passwordError.Text = "Пароль повинен мати від 8 до 20 символів.";                
            }
            else
            {
                passwordError.Text = "Пароль має містити комбінацію цифр та літер різних регістрів."; // Display an error message
            }
            errormessage.Text = "";
        }
        private void PasswordConfirmBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = (PasswordBox)sender;
            string password = passwordBox.Password;

            if (password != passwordBox1.Password)
            {
                confirmPassError.Text = "Паролі не співпадають.";
            }
            else
            {
                confirmPassError.Text = ""; // Display an error message
            }
            errormessage.Text = "";
        }
        private void textBoxFirstName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string firstName = textBoxFirstName.Text;

            if (string.IsNullOrEmpty(firstName))
            {
                firstNameError.Text = "Введіть ім'я.";  
            } 
            else if (firstName.Length < 2 || firstName.Length > 18)
            {
                firstNameError.Text = "Ім'я повинне мати від 2 до 18 символів.";                
            }
            else if (!Regex.IsMatch(firstName, "^(?:[A-Za-z]+|[А-ЩЬЮЯҐЄІЇа-щьюяґєії']+)$"))
            {
                firstNameError.Text = "Введіть коректне ім'я.";
            }
            else
            {
                firstNameError.Text = "";
            }
            errormessage.Text = "";
        }
        private void textBoxLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string lastName = textBoxLastName.Text;

            if (string.IsNullOrEmpty(lastName))
            {
                lastNameError.Text = "Введіть прізвище.";
            }
            else if (lastName.Length < 2 || lastName.Length > 18)
            {
                lastNameError.Text = "Прізвище повинне мати від 2 до 18 символів.";
            }
            else if (!Regex.IsMatch(lastName, "^(?:[A-Za-z]+|[А-ЩЬЮЯҐЄІЇа-щьюяґєії']+)$"))
            {
                lastNameError.Text = "Введіть коректне прізвище.";
            }
            else
            {
                lastNameError.Text = "";
            }
            errormessage.Text = "";
        }
        private void textBoxEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            string email = textBoxEmail.Text;

            if (string.IsNullOrEmpty(email))
            {
                emailError.Text = "Введіть електронну пошту.";  
            }
            else if (!IsEmailValid(email))
            {
                emailError.Text = "Введіть коректну електронну пошту.";
            }
            else{
                emailError.Text = "";
            }
            errormessage.Text = "";
        }
        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime? selectedDate = datePickerBirthdate.SelectedDate;

            if (selectedDate.HasValue)
            {
                // Перевірка чи дата народження не більше поточної дати
                if (selectedDate > DateTime.Now)
                {
                    dateError.Text = "Дата народження не може бути у майбутньому.";
                }
                else
                {
                    // Скинути повідомлення про помилку
                    dateError.Text = "";
                }
            }
            else
            {
                // Введена недійсна дата
                dateError.Text = "Введіть коректну дату народження.";
            }
            errormessage.Text = "";
        }

    }
}
