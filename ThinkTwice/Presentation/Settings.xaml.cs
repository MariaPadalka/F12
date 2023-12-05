using BLL;
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
using BLL;
using DAL;
using System.Globalization;
using System.Text.RegularExpressions;
using BLL.DTO;
using System.Reflection.Metadata;
using System.Collections.ObjectModel;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Settings : Page
    {
        private readonly string EmptyID ="1122F421-1716-410A-A1F2-334C3DC17096";
        private readonly SettingsService _settingsService = new SettingsService();
        public ObservableCollection<Category> categories;
        private ObservableCollection<Category> categoriesToDelete= new ObservableCollection<Category>();
        public Settings()
        {
            InitializeComponent();
            Loaded += YourWindow_Loaded;
            /*InitializeData();*/
        }


        private void OpenCreateCategoryWindow(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border?.Tag is Guid categoryId && categoryId == new Guid(EmptyID))
            {
                var createCategoryWindow = new CreateCategoryWindow(this);
                createCategoryWindow.ShowDialog();
            }
            

           
        }
        public void updateItems()
        {
            categories = new ObservableCollection<Category>(_settingsService.GetUserCategories(App.GetCurrentUser()));
            categories = new ObservableCollection<Category>(categories.OrderByDescending(category => category.UserId));
            itemsControl.ItemsSource = categories;
        }
        private Category emptyCategory()
        {
            Category empty = new Category();
            empty.Title = "Додати категорію";
            empty.IsGeneral = true;
            empty.Id = new Guid(EmptyID);

            return empty;
        }
        public void AddEmptyCategory()
        {
            // Перевірка, чи вже є категорія з таким ID
            if (categories.Any(category => category.Id == new Guid(EmptyID)))
            {
                // Категорія вже існує, нічого не робимо
                return;
            }

            Category empty = emptyCategory();
            categories.Add(empty);
            itemsControl.ItemsSource = categories;
        }

        private void RemoveEmptyCategory()
        {
            Category categoryToRemove = categories.FirstOrDefault(category => category.Id == new Guid(EmptyID));

            if (categoryToRemove != null)
            {
                categories.Remove(categoryToRemove);
                itemsControl.ItemsSource = categories;
            }
        }

        private void YourWindow_Loaded(object sender, RoutedEventArgs e)
        {
            textBoxName.Text = App.GetCurrentUser()?.Name;
            textBoxSurname.Text = App.GetCurrentUser()?.Surname;
            textBoxEmail.Text = App.GetCurrentUser()?.Email;
            datePickerBirthdate.Text = App.GetCurrentUser()?.BirthDate.ToString();
            textBoxCurrency.Text = App.GetCurrentUser()?.Currency;

            updateItems();
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
        private void ChangeClick(object sender, RoutedEventArgs e)
        {
            SaveButton.Visibility = Visibility.Visible;
            ChangeButton.Visibility = Visibility.Collapsed;
            textBoxName.IsEnabled = true;
            textBoxSurname.IsEnabled = true;
            textBoxEmail.IsEnabled = true;
            datePickerBirthdate.IsEnabled = true;
            AddEmptyCategory();
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement button && button.DataContext is Category category)
            {
                DeleteCategory(category);
            }
        }

        private void DeleteCategory(Category category)
        {
            categories.Remove(category);
            categoriesToDelete.Add(category);
            itemsControl.ItemsSource = categories;
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            var allFieldsValid = AllFieldsValid();
            if (allFieldsValid)
            {
                string email = textBoxEmail.Text;
                string first_name = textBoxName.Text;
                string last_name = textBoxSurname.Text;
                DateTime? date = datePickerBirthdate.SelectedDate;
                ICollection<Category> new_categories = (ICollection<Category>)categories;

                if (categories != null)
                {
                    new_categories = new_categories.Where(category => category.UserId != null).ToList();
                }
                UserDTO user = App.GetCurrentUser();
                user.Name = first_name;
                user.Surname = last_name;
                user.Email = email;
                user.BirthDate = date;
                user.Categories = new_categories;

                App.SetCurrentUser(user);
                _settingsService.UpdateUser(user);

                if (!categoriesToDelete.IsNullOrEmpty())
                {
                    foreach (var category in categoriesToDelete)
                    {
                        _settingsService.RemoveCategory(user, category.Id);
                    }
                }

                ChangeButton.Visibility = Visibility.Visible;
                SaveButton.Visibility = Visibility.Collapsed;
                textBoxName.IsEnabled = false;
                textBoxSurname.IsEnabled = false;
                textBoxEmail.IsEnabled = false;
                datePickerBirthdate.IsEnabled = false;
                RemoveEmptyCategory();
            }
        }
        private bool AllFieldsValid()
        {
            string error_mes = "";
            error_mes = textBoxFirstName_Error();
            if(error_mes == "") {
                error_mes = textBoxLastName_Error();
            } 
            
            if(error_mes == "")
            {
                error_mes = textBoxEmail_Error();
            } 
            
            if(error_mes == "")
            {
                error_mes = DatePicker_SelectedDateError();
            }

            errormessage.Text = error_mes;
            return errormessage.Text == "";
        }

        private string textBoxFirstName_Error()
        {
            string firstName = textBoxName.Text;

            if (string.IsNullOrEmpty(firstName))
            {
                return"Введіть ім'я.";
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
                return"";
            }
        }
        private string textBoxLastName_Error()
        {
            string lastName = textBoxSurname.Text;

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
                return "";
            }
        }
        public static bool IsEmailValid(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            string emailPattern = @"^[a-zA-Z0-9._%+-]{3,20}@[a-zA-Z0-9.-]{2,20}\.[a-zA-Z]{2,10}$";

            return Regex.IsMatch(email, emailPattern);
        }
        private string textBoxEmail_Error()
        {
            string email = textBoxEmail.Text;

            if (string.IsNullOrEmpty(email))
            {
                return "Введіть електронну пошту.";
            }
            else if (!IsEmailValid(email))
            {
                return "Введіть коректну електронну пошту.";
            }else if (email != App.GetCurrentUser()?.Email && !_settingsService.UniqueEmail(email))
            {
                return "Користувач з такою поштою вже існує";
            }
            else
            {
                return "";
            }
        }

        private string DatePicker_SelectedDateError()
        {
            DateTime? selectedDate = datePickerBirthdate.SelectedDate;

            if (selectedDate.HasValue)
            {
                DateTime twelveYearsAgo = DateTime.Now.AddYears(-12);

                if (selectedDate > twelveYearsAgo)
                {
                    return "Користувач повинен бути старше 12 років.";
                }
                else
                {
                    return "";
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
                        return Brushes.Red; // Колір для Витрат
                    case "Дохід":
                        return Brushes.Green; // Колір для Доходу
                    case "Баланс":
                        return Brushes.Blue; // Колір для Балансу
                    default:
                        return Brushes.Gray; // Колір за замовчуванням
                }
            }

            return Brushes.Gray; // Колір за замовчуванням
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class GuidToBackgroundConverter : IValueConverter
    {
        private readonly string EmptyID = "1122F421-1716-410A-A1F2-334C3DC17096";
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Guid id && id == new Guid(EmptyID))
            {
                return new SolidColorBrush(Color.FromRgb(64, 102, 125));
            }

            // Повернути інший колір, якщо id не відповідає вашому умові
            return new SolidColorBrush(Color.FromRgb(207, 217, 227));
        }//;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class GuidToForegroundConverter : IValueConverter
    {
                private readonly string EmptyID = "1122F421-1716-410A-A1F2-334C3DC17096";
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Guid id && id == new Guid(EmptyID))
            {
                return Brushes.White;
            }

            // Повернути інший колір, якщо id не відповідає вашому умові
            return Brushes.Black;
        }//;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
