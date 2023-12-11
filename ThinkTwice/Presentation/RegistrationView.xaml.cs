namespace Presentation
{
    using System;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Navigation;
    using BLL;
    using Serilog;
    using ThinkTwice_Context;

    public partial class RegistrationView : Page
    {
        private readonly ILogger logger = LoggerManager.Instance.Logger;

        private RegistrationService registrationService = new RegistrationService();

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationView"/> class.
        /// </summary>
        public RegistrationView()
        {
            this.InitializeComponent();

            this.logger.Information("Перехід на сторінку реєстрації.");
        }

        public static bool IsEmailValid(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            string emailPattern = @"^[a-zA-Z0-9._%+-]{3,20}@[a-zA-Z0-9.-]{2,20}\.[a-zA-Z]{2,10}$";

            return Regex.IsMatch(email, emailPattern);
        }

        public static bool IsPasswordValid(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            string passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,20}$";

            return Regex.IsMatch(password, passwordPattern);
        }

        public void Reset()
        {
            this.textBoxFirstName.Text = string.Empty;
            this.textBoxLastName.Text = string.Empty;
            this.textBoxEmail.Text = string.Empty;
            this.passwordBox1.Password = string.Empty;
            this.passwordBoxConfirm.Password = string.Empty;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new Uri("Login.xaml", UriKind.Relative));
        }

        private void GoToDashboard(object sender, RoutedEventArgs e)
        {
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new Uri("Dashboard.xaml", UriKind.Relative));
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.Submit_Click(sender, e);
            }
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            string email = this.textBoxEmail.Text;
            string password = this.passwordBox1.Password;
            string first_name = this.textBoxFirstName.Text;
            string last_name = this.textBoxLastName.Text;
            DateTime? date = this.datePickerBirthdate.SelectedDate;
            string currency = this.comboBoxCurrency.Text;
            bool all_fields_present = email != string.Empty && password != string.Empty && first_name != string.Empty && last_name != string.Empty && date.HasValue && currency != string.Empty;

            if (!all_fields_present || this.passwordError.Text != string.Empty || this.confirmPassError.Text != string.Empty || this.firstNameError.Text != string.Empty || this.lastNameError.Text != string.Empty || this.emailError.Text != string.Empty || this.dateError.Text != string.Empty)
            {
                this.errormessage.Text = "Будь ласка, введіть коректні дані у всіх полях.";
                this.logger.Error("Помилка при створенні користувача.");
            }
            else
            {
                var user = this.registrationService.Register(email, password, first_name, last_name, date, currency);
                if (user != null)
                {
                    this.errormessage.Text = "Ви успішно зареєструвались.";
                    App.SetCurrentUser(user);
                    this.logger.Information($"Користувача {App.CurrentUser.Name} {App.CurrentUser.Surname} успішно створено.");
                    this.GoToDashboard(sender, e);
                }
                else
                {
                    this.errormessage.Text = "Ця пошта вже комусь належить.";
                    this.logger.Error("Помилка при створенні користувача.");
                }
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = (PasswordBox)sender;
            string password = passwordBox.Password;

            if (IsPasswordValid(password))
            {
                this.passwordError.Text = string.Empty;
            }
            else if (password.Length < 8 || password.Length > 20)
            {
                this.passwordError.Text = "Пароль повинен мати від 8 до 20 символів.";
                this.logger.Error("Помилка при створенні користувача.");
            }
            else
            {
                this.passwordError.Text = "Пароль має містити комбінацію цифр та літер різних регістрів.        ";
                this.logger.Error("Помилка при створенні користувача.");
            }

            this.errormessage.Text = string.Empty;
        }

        private void TextBoxPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            string password = this.textBoxPassword.Text;

            if (IsPasswordValid(password))
            {
                this.passwordError.Text = string.Empty;
            }
            else if (password.Length < 8 || password.Length > 20)
            {
                this.passwordError.Text = "Пароль повинен мати від 8 до 20 символів.";
                this.logger.Error("Помилка при створенні користувача.");
            }
            else
            {
                this.passwordError.Text = "Пароль має містити комбінацію цифр та літер різних регістрів.        ";
                this.logger.Error("Помилка при створенні користувача.");
            }

            this.errormessage.Text = string.Empty;
        }

        private void PasswordConfirmBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = (PasswordBox)sender;
            string password = passwordBox.Password;

            if (password != this.passwordBox1.Password)
            {
                this.confirmPassError.Text = "Паролі не співпадають.";
                this.logger.Error("Помилка при створенні користувача.");
            }
            else
            {
                this.confirmPassError.Text = string.Empty;
            }

            this.errormessage.Text = string.Empty;
        }

        private void TextBoxConfirmPassword_TextChanged(object sender, TextChangedEventArgs e)
        {
            string password = this.textBoxConfirmPassword.Text;

            if (password != this.passwordBox1.Password)
            {
                this.confirmPassError.Text = "Паролі не співпадають.";
                this.logger.Error("Помилка при створенні користувача.");
            }
            else
            {
                this.confirmPassError.Text = string.Empty;
            }

            this.errormessage.Text = string.Empty;
        }

        private void TextBoxFirstName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string firstName = this.textBoxFirstName.Text;

            if (string.IsNullOrEmpty(firstName))
            {
                this.firstNameError.Text = "Введіть ім'я.";
                this.logger.Warning("Не заповнено обов'язкові поля");
            }
            else if (firstName.Length < 2 || firstName.Length > 18)
            {
                this.firstNameError.Text = "Ім'я повинне мати від 2 до 18 символів.";
                this.logger.Error("Помилка при створенні користувача.");
            }
            else if (!Regex.IsMatch(firstName, "^(?:[A-Za-z-]+|[А-ЩЬЮЯҐЄІЇа-щьюяґєії'-]+)$"))
            {
                this.firstNameError.Text = "Введіть коректне ім'я.";
                this.logger.Error("Помилка при створенні користувача.");
            }
            else
            {
                this.firstNameError.Text = string.Empty;
            }

            this.errormessage.Text = string.Empty;
        }

        private void TextBoxLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string lastName = this.textBoxLastName.Text;

            if (string.IsNullOrEmpty(lastName))
            {
                this.lastNameError.Text = "Введіть прізвище.";
                this.logger.Warning("Не заповнено обов'язкові поля");
            }
            else if (lastName.Length < 2 || lastName.Length > 18)
            {
                this.lastNameError.Text = "Прізвище повинне мати від 2 до 18 символів.";
                this.logger.Error("Помилка при створенні користувача.");
            }
            else if (!Regex.IsMatch(lastName, "^(?:[A-Za-z-]+|[А-ЩЬЮЯҐЄІЇа-щьюяґєії'-]+)$"))
            {
                this.lastNameError.Text = "Введіть коректне прізвище.";
                this.logger.Error("Помилка при створенні користувача.");
            }
            else
            {
                this.lastNameError.Text = string.Empty;
            }

            this.errormessage.Text = string.Empty;
        }

        private void TextBoxEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            string email = this.textBoxEmail.Text;

            if (string.IsNullOrEmpty(email))
            {
                this.emailError.Text = "Введіть електронну пошту.";
                this.logger.Warning("Не заповнено обов'язкові поля");
            }
            else if (!IsEmailValid(email))
            {
                this.emailError.Text = "Введіть коректну електронну пошту.";
                this.logger.Error("Помилка при створенні користувача.");
            }
            else
            {
                this.emailError.Text = string.Empty;
            }

            this.errormessage.Text = string.Empty;
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime? selectedDate = this.datePickerBirthdate.SelectedDate;

            if (selectedDate.HasValue)
            {
                DateTime twelveYearsAgo = DateTime.Now.AddYears(-12);

                if (selectedDate > twelveYearsAgo)
                {
                    this.dateError.Text = "Користувач повинен бути старше 12 років.";
                    this.logger.Fatal("Клієнт не належить до дозволеної вікової категорії користувачів.");
                }
                else
                {
                    this.dateError.Text = string.Empty;
                }
            }
            else
            {
                this.dateError.Text = "Введіть коректну дату народження.";
                this.logger.Error("Помилка при створенні користувача.");
            }

            this.errormessage.Text = string.Empty;
        }

        private void ShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            this.passwordBox1.Visibility = Visibility.Collapsed;
            this.textBoxPassword.Visibility = Visibility.Visible;
            this.textBoxPassword.Text = this.passwordBox1.Password;
            this.toggleButtonShowPassword.IsChecked = true;
        }

        private void ShowPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            this.passwordBox1.Visibility = Visibility.Visible;
            this.textBoxPassword.Visibility = Visibility.Collapsed;
            this.passwordBox1.Password = this.textBoxPassword.Text;
            this.textBoxPassword.Text = string.Empty;
            this.toggleButtonShowPassword.IsChecked = false;
        }

        private void ShowConfirmPassword_Checked(object sender, RoutedEventArgs e)
        {
            this.passwordBoxConfirm.Visibility = Visibility.Collapsed;
            this.textBoxConfirmPassword.Visibility = Visibility.Visible;
            this.textBoxConfirmPassword.Text = this.passwordBoxConfirm.Password;
            this.toggleButtonShowConfirmPassword.IsChecked = true;
        }

        private void ShowConfirmPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            this.passwordBoxConfirm.Visibility = Visibility.Visible;
            this.textBoxConfirmPassword.Visibility = Visibility.Collapsed;
            this.passwordBoxConfirm.Password = this.textBoxConfirmPassword.Text;
            this.textBoxConfirmPassword.Text = string.Empty;
            this.toggleButtonShowConfirmPassword.IsChecked = false;
        }
    }
}
