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
using Presentation.DTO;
using Microsoft.EntityFrameworkCore;
using BLL.DTO;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Transactions : Page
    {
        public readonly TransactionService _transactionService = new TransactionService();
        public readonly CategoryRepository _categoryRepository = new CategoryRepository();
        public readonly CategoryService _categoryService = new CategoryService();


        public Transactions()
        {
            InitializeComponent();
            Loaded += YourWindow_Loaded;

            ComboBox_Source();
            ComboBox_Destination();
            DatePickerBorder.Visibility = Visibility.Collapsed;
        }

        private void YourWindow_Loaded(object sender, RoutedEventArgs e)
        {
            List<Transaction>? defaultData = _transactionService.GetTransactions(App.GetCurrentUser())?.Where(c => c.Date < DateTime.Now.Date || (c.Date == DateTime.Now.Date && c.Planned == false)).ToList();
            List<TransactionDTO> data = new List<TransactionDTO>();
            foreach (Transaction transaction in defaultData)
            {
                if (transaction != null)
                {
                    data.Add(new TransactionDTO(transaction));
                }
            }
            dataGrid.ItemsSource = data;


            
            List<Transaction>? plannedData = _transactionService.GetTransactions(App.GetCurrentUser())?.Where(c => (c.Date > DateTime.Now.Date && c.Planned == true)).ToList();
            List<TransactionDTO> rows = new List<TransactionDTO>();
            foreach (Transaction transaction in plannedData)
            {
                if (transaction != null)
                {
                    rows.Add(new TransactionDTO(transaction));
                }
            }
            dataGridPlannedTransactions.ItemsSource = rows;
        }

        public void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new Uri("Dashboard.xaml", UriKind.Relative));
        }
        public void Settings_Click(object sender, RoutedEventArgs e)
        {
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new Uri("Settings.xaml", UriKind.Relative));
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

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public void ComboBox_Source()
        {
            List<string>? arr1 = _categoryService.GetCategoriesTitleByType(App.GetCurrentUser(), "Баланс");
            List<string>? arr2 = _categoryService.GetCategoriesTitleByType(App.GetCurrentUser(), "Дохід");

            List<string> combinedList = new List<string>(arr1);
            combinedList.AddRange(arr2);

            sourceComboBox.ItemsSource = combinedList;
        }

        public void ComboBox_Destination()
        {
            List<string>? arr1 = _categoryService.GetCategoriesTitleByType(App.GetCurrentUser(), "Баланс");
            List<string>? arr2 = _categoryService.GetCategoriesTitleByType(App.GetCurrentUser(), "Витрати");

            List<string>? combinedList = new List<string>(arr1);
            combinedList.AddRange(arr2);

            destinationComboBox.ItemsSource = combinedList;
        }

        private void CreateTransactionButton_Click(object sender, RoutedEventArgs e)
        {
            var all_fields_valid = AllFieldsValid();
            if (all_fields_valid)
            {
                string category_from_title = (string)sourceComboBox.SelectedValue;
                string category_to_title = (string)destinationComboBox.SelectedValue;
                decimal amount = decimal.Parse(amountTextBox.Text);
                string details = detailsTextBox.Text;
                bool planned = checkboxIsPlanned.IsChecked.HasValue && checkboxIsPlanned.IsChecked.Value;
                
                DateTime? date = new DateTime();


                Guid? category_from = _categoryRepository.GetCategoryByName(App.GetCurrentUser().Id, category_from_title).Id;
                Guid? category_to = _categoryRepository.GetCategoryByName(App.GetCurrentUser().Id, category_to_title).Id;


                if (!planned) { date = DateTime.Now; }
                else { date = (DateTime)datePickerPlannedDate.SelectedDate.Value; }

                if (( _categoryRepository.GetCategoryById(category_from).Type == "Баланс" || _categoryRepository.GetCategoryById(category_from).Type == "Дохід" ) && _categoryRepository.GetCategoryById(category_to).Type == "Витрати")
                {
                    amount *= -1;
                }

                _transactionService.AddTransaction(App.GetCurrentUser(), category_to, category_from, amount, date, details, planned);


                if (planned)
                {
                    List<Transaction>? plannedData = _transactionService.GetTransactions(App.GetCurrentUser())?.Where(c => (c.Date > DateTime.Now.Date && c.Planned == true)).ToList();
                    List<TransactionDTO> rows = new List<TransactionDTO>();
                    foreach (Transaction transaction in plannedData)
                    {
                        if (transaction != null)
                        {
                            rows.Add(new TransactionDTO(transaction));
                        }
                    }
                    dataGridPlannedTransactions.ItemsSource = rows;
                }
                else
                {
                    List<Transaction>? defaultData = _transactionService.GetTransactions(App.GetCurrentUser())?.Where(c => c.Date < DateTime.Now.Date || (c.Date?.Date == DateTime.Now.Date && c.Planned == false)).ToList();
                    List<TransactionDTO> data = new List<TransactionDTO>();
                    foreach (Transaction transaction in defaultData)
                    {
                        if (transaction != null)
                        {
                            data.Add(new TransactionDTO(transaction));
                        }
                    }
                    dataGrid.ItemsSource = data;
                }

                sourceComboBox.SelectedIndex = -1;
                destinationComboBox.SelectedIndex = -1;
                detailsTextBox.Text = string.Empty;
                amountTextBox.Text = string.Empty;
                datePickerPlannedDate.SelectedDate = null;
            }
            
        }

        private bool AllFieldsValid()
        {
            string error_mes = "";
            error_mes = SourceComboBox_Error();

            if (error_mes == "")
            {
                error_mes = DestinationComboBox_Error();
            }

            if (error_mes == "")
            {
                error_mes = AmountTextBox_Error();
            }

            if (error_mes == "")
            {
                error_mes = DatePickerPlannedDate_Error();
            }

            errormessage.Text = error_mes;
            return errormessage.Text == "";
        }
        private string SourceComboBox_Error()
        {
            string category_from_title = (string)sourceComboBox.SelectedValue;
            
            if (string.IsNullOrEmpty(category_from_title))
            {
                return "Оберіть джерело.";
            }
            else
            {
                return "";
            }
        }
        private string DestinationComboBox_Error() 
        {
            string category_to_title = (string)destinationComboBox.SelectedValue;

            if (string.IsNullOrEmpty(category_to_title))
            {
                return "Оберіть призначення";
            }
            else
            {
                return "";
            }
        }
        private string AmountTextBox_Error()
        {
            if (string.IsNullOrWhiteSpace(amountTextBox.Text))
            {
                return "Введіть суму.";
            }
            else if (!decimal.TryParse(amountTextBox.Text, out decimal result))
            {
                return "Некоректний формат суми.";
            }
            else
            {
                return "";
            }
        }
        private string DatePickerPlannedDate_Error()
        {
            if (checkboxIsPlanned.IsChecked.HasValue && checkboxIsPlanned.IsChecked.Value)
            {
                if (string.IsNullOrWhiteSpace(datePickerPlannedDate.Text))
                {
                    return "Оберіть дату.";
                }
                else if (!DateTime.TryParse(datePickerPlannedDate.Text, out DateTime selectedDate))
                {
                    return "Оберіть коректну дату";
                }
                else if (selectedDate.Date < DateTime.Today)
                {
                    return "Дата не може бути у минулому";
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            DatePickerBorder.Visibility = Visibility.Visible;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            DatePickerBorder.Visibility = Visibility.Collapsed;
        }




        // Допоміжний метод для отримання ScrollViewer для DataGrid
        private ScrollViewer GetScrollViewer(DependencyObject depObj)
        {
            if (depObj is ScrollViewer viewer)
                return viewer;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                var result = GetScrollViewer(child);
                if (result != null)
                    return result;
            }

            return null;
        }


        private void DataGridPlannedScrollBackward_Click(object sender, RoutedEventArgs e)
        {
            var scrollViewer = GetScrollViewer(dataGridPlannedTransactions);

            double jumpSize = Math.Floor(scrollViewer.ViewportHeight);

            scrollViewer.ScrollToVerticalOffset(Math.Max(0, scrollViewer.VerticalOffset - jumpSize));
        }

        private void DataGridPlannedScrollForward_Click(object sender, RoutedEventArgs e)
        {
            var scrollViewer = GetScrollViewer(dataGridPlannedTransactions);

            double jumpSize = Math.Floor(scrollViewer.ViewportHeight);

            scrollViewer.ScrollToVerticalOffset(Math.Min(scrollViewer.ScrollableHeight, scrollViewer.VerticalOffset + jumpSize));
        }

        private void DataGridPlannedTransactions_Loaded(object sender, RoutedEventArgs e)
        {
            ScrollViewer scrollViewer = GetScrollViewer(dataGridPlannedTransactions);
            if (scrollViewer != null)
            {
                scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            }
        }



        private void DataGridGeneralScrollBackward_Click(object sender, RoutedEventArgs e)
        {
            var scrollViewer = GetScrollViewer(dataGrid);

            double jumpSize = Math.Floor(scrollViewer.ViewportHeight);

            scrollViewer.ScrollToVerticalOffset(Math.Max(0, scrollViewer.VerticalOffset - jumpSize));
        }

        private void DataGridGeneralScrollForward_Click(object sender, RoutedEventArgs e)
        {
            var scrollViewer = GetScrollViewer(dataGrid);

            double jumpSize = Math.Floor(scrollViewer.ViewportHeight);

            scrollViewer.ScrollToVerticalOffset(Math.Min(scrollViewer.ScrollableHeight, scrollViewer.VerticalOffset + jumpSize));
        }

        private void DataGridGeneralTransactions_Loaded(object sender, RoutedEventArgs e)
        {
            ScrollViewer scrollViewer = GetScrollViewer(dataGrid);
            if (scrollViewer != null)
            {
                scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            }
        }

        private void DataGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            e.Handled = true; // Заборонити обробку скролінгу миші
        }

        private void dataGridPlannedTransactions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void dataGrid_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void dataGrid_SelectionChanged_2(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Run_DragOver(object sender, DragEventArgs e)
        {

        }
    }
}
