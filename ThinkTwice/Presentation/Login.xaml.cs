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

    public partial class Login : Page
    {
        private readonly ILogger logger = LoggerManager.Instance.Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Login"/> class.
        /// </summary>
        public Login()
        {
            this.InitializeComponent();

            this.logger.Information("Перехід на сторінку авторизації.");
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

        private void Registration_Click(object sender, RoutedEventArgs e)
        {
           
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new Uri("RegistrationView.xaml", UriKind.Relative));
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
            string password;
            if (this.textBoxPassword.Visibility == Visibility.Visible)
            {
                password = this.textBoxPassword.Text;
            }
            else
            {
                password = this.passwordBox1.Password;
            }

            AuthenticationService authenticationService = new AuthenticationService();

            if (!IsEmailValid(email))
            {
                this.errormessage.Text = "Введіть коректну електронну пошту.";
                this.logger.Error("Помилка авторизації.");
            }
            else
            {
                var user = authenticationService.AuthenticateUser(email, password);
                if (user == null)
                {
                    this.errormessage.Text = "Користувача не знайдено.";
                    this.logger.Error("Помилка авторизації.");
                }
                else
                {
                    this.errormessage.Text = string.Empty;
                    App.SetCurrentUser(user);
                    this.logger.Information($"Користувача {App.CurrentUser.Name} {App.CurrentUser.Surname} авторизовано.");
                    this.GoToDashboard(sender, e);
                }
            }
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
    }
}
