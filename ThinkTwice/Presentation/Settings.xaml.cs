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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ThinkTwice_Context;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Settings : Page
    {
        public Settings()
        {
            InitializeComponent();
            Loaded += YourWindow_Loaded;
            /*InitializeData();*/


        }

        private void YourWindow_Loaded(object sender, RoutedEventArgs e)
        {
            textBoxName.Text = App.GetCurrentUser()?.Name;
            textBoxSurname.Text = App.GetCurrentUser()?.Surname;
            textBoxEmail.Text = App.GetCurrentUser()?.Email;
            datePickerBirthdate.Text = App.GetCurrentUser()?.BirthDate.ToString();
            comboBoxCurrency.Text = App.GetCurrentUser()?.Currency;
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public void Transactions_Click(object sender, RoutedEventArgs e)
        {
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new Uri("Transactions.xaml", UriKind.Relative));
        }
        public void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new Uri("Dashboard.xaml", UriKind.Relative));
        }
        public void Logout(object sender, RoutedEventArgs e)
        {
            App.RemoveUser();
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new Uri("Login.xaml", UriKind.Relative));
        }
        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            //DateTime? selectedDate = datePickerBirthdate.SelectedDate;

            //if (selectedDate.HasValue)
            //{
            //    if (selectedDate > DateTime.Now)
            //    {
            //        dateError.Text = "Дата народження не може бути у майбутньому.";
            //    }
            //    else
            //    {
            //        dateError.Text = "";
            //    }
            //}
            //else
            //{
            //    dateError.Text = "Введіть коректну дату народження.";
            //}
            //errormessage.Text = "";
        }
        private void ShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            passwordBox1.Visibility = Visibility.Collapsed;
            textBoxPassword.Visibility = Visibility.Visible;
            PasswordBorder.Visibility = Visibility.Visible;
            textBoxPassword.Text = passwordBox1.Password;
            toggleButtonShowPassword.IsChecked = true;
        }

        private void ShowPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            passwordBox1.Visibility = Visibility.Visible;
            textBoxPassword.Visibility = Visibility.Collapsed;
            PasswordBorder.Visibility = Visibility.Collapsed;
            passwordBox1.Password = textBoxPassword.Text;
            textBoxPassword.Text = string.Empty;
            toggleButtonShowPassword.IsChecked = false;
        }
    }
}
