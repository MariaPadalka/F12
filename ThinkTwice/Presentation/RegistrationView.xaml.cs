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

namespace Presentation
{
    /// <summary>
    /// Interaction logic for RegistrationView.xaml
    /// </summary>
    public partial class RegistrationView : Page
    {
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
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }
        public void Reset()
        {
            textBoxFirstName.Text = "";
            textBoxLastName.Text = "";
            textBoxEmail.Text = "";
            passwordBox1.Password = "";
            passwordBoxConfirm.Password = "";
        }
        private void button3_Click(object sender, RoutedEventArgs e)
        {
            //Close();
        }
        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxEmail.Text.Length == 0)
            {
                errormessage.Text = "Введіть електронну пошту.";
                textBoxEmail.Focus();
            }
            else if (!IsEmailValid(textBoxEmail.Text))
            {
                errormessage.Text = "Введіть валідну електронну пошту.";
                textBoxEmail.Select(0, textBoxEmail.Text.Length);
                textBoxEmail.Focus();
            }
            else
            {
                string lastname = textBoxLastName.Text;
                string firstname = textBoxFirstName.Text;
                string email = textBoxEmail.Text;
                string password = passwordBox1.Password;

                errormessage.Text = "";
                SqlConnection con = new SqlConnection("Data Source=TESTPURU;Initial Catalog=Data;User ID=sa;Password=wintellect");
                con.Open();
                //SqlCommand cmd = new SqlCommand("Insert into Registration (FirstName,LastName,Email,Password,Address) values('" + firstName + "','" + lastname + "','" + email + "','" + password + "','" + address + "')", con);
                //cmd.CommandType = CommandType.Text;
                //cmd.ExecuteNonQuery();
                con.Close();
                errormessage.Text = "Ви успішно зареєструвались.";
                Reset();
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
                passwordError.Text = "Пароль повинен містити принаймні одну малу літеру, одну велику літеру та одну цифру"; // Display an error message
            }
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
            else if (!Regex.IsMatch(firstName, "^[a-zA-Z&]*$") && !Regex.IsMatch(firstName, "[а-яА-ЯїЇєЄґҐіІ\']"))
            {
                firstNameError.Text = "Введіть коректне ім'я.";
            }
            else
            {
                firstNameError.Text = "";
            }
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
            else if (!Regex.IsMatch(lastName, "^[a-zA-Z&]*$") && !Regex.IsMatch(lastName, "[а-яА-ЯїЇєЄґҐіІ\']"))
            {
                lastNameError.Text = "Введіть коректне прізвище.";
            }
            else 
            {
                lastNameError.Text = "";
            }
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
        }

    }
}
