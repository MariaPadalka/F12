namespace Presentation
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Navigation;
    using BLL;
    using BLL.DTO;
    using Microsoft.IdentityModel.Tokens;
    using ThinkTwice_Context;

    /// <summary>
    /// Interaction logic for Dashboard.xaml.
    /// </summary>
    public partial class Settings : Page
    {
        private readonly SettingsService settingsService = new SettingsService();
        private ObservableCollection<Category> categories;
        private ObservableCollection<Category> categoriesToDelete = new ObservableCollection<Category>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        public Settings()
        {
            this.InitializeComponent();
            this.Loaded += this.YourWindow_Loaded;
            /*InitializeData();*/
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

        public void TransactionsClick(object sender, RoutedEventArgs e)
        {
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new Uri("Transactions.xaml", UriKind.Relative));
        }

        public void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new Uri("Dashboard.xaml", UriKind.Relative));
        }

        public void Statistics_Click(object sender, RoutedEventArgs e)
        {
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new Uri("Statistics.xaml", UriKind.Relative));
        }

        public void Logout(object sender, RoutedEventArgs e)
        {
            App.RemoveUser();
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new Uri("Login.xaml", UriKind.Relative));
        }

        private void OpenCreateCategoryWindow(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border?.Tag is Guid categoryId && categoryId == new Guid("1122F421-1716-410A-A1F2-334C3DC17096"))
            {
                var createCategoryWindow = new CreateCategoryWindow();
                createCategoryWindow.ShowDialog();
            }
        }

        private Category EmptyCategory()
        {
            Category empty = new Category();
            empty.Title = "Додати категорію";
            empty.IsGeneral = true;
            empty.Id = new Guid("1122F421-1716-410A-A1F2-334C3DC17096");

            return empty;
        }

        private void YourWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.textBoxName.Text = App.GetCurrentUser()?.Name;
            this.textBoxSurname.Text = App.GetCurrentUser()?.Surname;
            this.textBoxEmail.Text = App.GetCurrentUser()?.Email;
            this.datePickerBirthdate.Text = App.GetCurrentUser()?.BirthDate.ToString();
            this.textBoxCurrency.Text = App.GetCurrentUser()?.Currency;

            this.categories = new ObservableCollection<Category>(this.settingsService.GetUserCategories(App.GetCurrentUser()));
            this.categories = new ObservableCollection<Category>(this.categories.OrderByDescending(category => category.UserId));
            Category empty = this.EmptyCategory();
            this.categories.Add(empty);
            this.itemsControl.ItemsSource = this.categories;
        }

        private void ChangeClick(object sender, RoutedEventArgs e)
        {
            this.SaveButton.Visibility = Visibility.Visible;
            this.ChangeButton.Visibility = Visibility.Collapsed;
            this.textBoxName.IsEnabled = true;
            this.textBoxSurname.IsEnabled = true;
            this.textBoxEmail.IsEnabled = true;
            this.datePickerBirthdate.IsEnabled = true;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement button && button.DataContext is Category category)
            {
                this.DeleteCategory(category);
            }
        }

        private void DeleteCategory(Category category)
        {
            this.categories.Remove(category);
            this.categoriesToDelete.Add(category);
            Category empty = this.EmptyCategory();
            this.categories.Add(empty);
            this.itemsControl.ItemsSource = this.categories;
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            var allFieldsValid = this.AllFieldsValid();
            if (allFieldsValid)
            {
                string email = this.textBoxEmail.Text;
                string first_name = this.textBoxName.Text;
                string last_name = this.textBoxSurname.Text;
                DateTime? date = this.datePickerBirthdate.SelectedDate;
                ICollection<Category> new_categories = (ICollection<Category>)this.categories;

                if (this.categories != null)
                {
                    new_categories = new_categories.Where(category => category.UserId != null).ToList();
                }

                UserDTO? user = App.GetCurrentUser();
                user.Name = first_name;
                user.Surname = last_name;
                user.Email = email;
                user.BirthDate = date;
                user.Categories = new_categories;

                App.SetCurrentUser(user);
                this.settingsService.UpdateUser(user);

                if (!this.categoriesToDelete.IsNullOrEmpty())
                {
                    foreach (var category in categoriesToDelete)
                    {
                        this.settingsService.RemoveCategory(user, category.Id);
                    }
                }

                this.ChangeButton.Visibility = Visibility.Visible;
                this.SaveButton.Visibility = Visibility.Collapsed;
                this.textBoxName.IsEnabled = false;
                this.textBoxSurname.IsEnabled = false;
                this.textBoxEmail.IsEnabled = false;
                this.datePickerBirthdate.IsEnabled = false;
            }
        }

        private bool AllFieldsValid()
        {
            string error_mes = string.Empty;
            error_mes = this.TextBoxFirstName_Error();
            if (error_mes == string.Empty)
            {
                error_mes = this.TextBoxLastName_Error();
            }

            if (error_mes == string.Empty)
            {
                error_mes = this.TextBoxEmail_Error();
            }

            if (error_mes == string.Empty)
            {
                error_mes = this.DatePicker_SelectedDateError();
            }

            this.errormessage.Text = error_mes;
            return this.errormessage.Text == string.Empty;
        }

        private string TextBoxFirstName_Error()
        {
            string firstName = this.textBoxName.Text;

            if (string.IsNullOrEmpty(firstName))
            {
                return "Введіть ім'я.";
            }
            else if (firstName.Length < 2 || firstName.Length > 18)
            {
                return "Ім'я повинне мати від 2 до 18 символів.";
            }
            else if (!Regex.IsMatch(firstName, "^(?:[A-Za-z-]+|[А-ЩЬЮЯҐЄІЇа-щьюяґєії'-]+)$"))
            {
                return "Введіть коректне ім'я.";
            }
            else
            {
                return string.Empty;
            }
        }

        private string TextBoxLastName_Error()
        {
            string lastName = this.textBoxSurname.Text;

            if (string.IsNullOrEmpty(lastName))
            {
                return "Введіть прізвище.";
            }
            else if (lastName.Length < 2 || lastName.Length > 18)
            {
                return "Прізвище повинне мати від 2 до 18 символів.";
            }
            else if (!Regex.IsMatch(lastName, "^(?:[A-Za-z-]+|[А-ЩЬЮЯҐЄІЇа-щьюяґєії'-]+)$"))
            {
                return "Введіть коректне прізвище.";
            }
            else
            {
                return string.Empty;
            }
        }

        private string TextBoxEmail_Error()
        {
            string email = this.textBoxEmail.Text;

            if (string.IsNullOrEmpty(email))
            {
                return "Введіть електронну пошту.";
            }
            else if (!IsEmailValid(email))
            {
                return "Введіть коректну електронну пошту.";
            }
            else if (email != App.GetCurrentUser()?.Email && !this.settingsService.UniqueEmail(email))
            {
                return "Користувач з такою поштою вже існує";
            }
            else
            {
                return string.Empty;
            }
        }

        private string DatePicker_SelectedDateError()
        {
            DateTime? selectedDate = this.datePickerBirthdate.SelectedDate;

            if (selectedDate.HasValue)
            {
                DateTime twelveYearsAgo = DateTime.Now.AddYears(-12);

                if (selectedDate > twelveYearsAgo)
                {
                    return "Користувач повинен бути старше 12 років.";
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return "Введіть коректну дату народження.";
            }
        }
    }

    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CategoryTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string categoryType)
            {
                switch (categoryType)
                {
                    case "Витрати":
                        return Brushes.Red;
                    case "Дохід":
                        return Brushes.Green;
                    case "Баланс":
                        return Brushes.Blue;
                    default:
                        return Brushes.Gray;
                }
            }

            return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
