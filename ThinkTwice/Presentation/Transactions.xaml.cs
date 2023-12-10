namespace Presentation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Navigation;
    using BLL;
    using Presentation.DTO;
    using ThinkTwice_Context;

    /// <summary>
    /// Interaction logic for Dashboard.xaml.
    /// </summary>
    /// 

    public partial class Transactions : Page
    {
        private readonly TransactionService transactionService = new TransactionService();
        private readonly CategoryRepository categoryRepository = new CategoryRepository();
        private readonly CategoryService categoryService = new CategoryService();

        /// <summary>
        /// Initializes a new instance of the <see cref="Transactions"/> class.
        /// </summary>
        public Transactions()
        {
            this.InitializeComponent();
            this.Loaded += this.YourWindow_Loaded;

            this.ComboBox_Source();
            this.ComboBox_Destination();
            this.DatePickerBorder.Visibility = Visibility.Collapsed;
        }

        public void DashboardClick(object sender, RoutedEventArgs e)
        {
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new Uri("Dashboard.xaml", UriKind.Relative));
        }

        public void SettingsClick(object sender, RoutedEventArgs e)
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

        public void ComboBox_Source()
        {
            List<string>? arr1 = this.categoryService.GetCategoriesTitleByType(App.GetCurrentUser(), "Баланс");
            List<string>? arr2 = this.categoryService.GetCategoriesTitleByType(App.GetCurrentUser(), "Дохід");

            List<string> combinedList = new List<string>(arr1);
            combinedList.AddRange(arr2);

            this.sourceComboBox.ItemsSource = combinedList;
        }

        public void ComboBox_Destination()
        {
            List<string>? arr1 = this.categoryService.GetCategoriesTitleByType(App.GetCurrentUser(), "Баланс");
            List<string>? arr2 = this.categoryService.GetCategoriesTitleByType(App.GetCurrentUser(), "Витрати");

            List<string>? combinedList = new List<string>(arr1);
            combinedList.AddRange(arr2);

            this.destinationComboBox.ItemsSource = combinedList;
        }

        private void YourWindow_Loaded(object sender, RoutedEventArgs e)
        {
            List<Transaction>? defaultData = this.transactionService.GetTransactions(App.GetCurrentUser())?.Where(c => c.Date < DateTime.Now.Date || (c.Date == DateTime.Now.Date && c.Planned == false)).ToList();
            List<TransactionDTO> data = new List<TransactionDTO>();
            foreach (Transaction transaction in defaultData)
            {
                if (transaction != null)
                {
                    //if ((this.categoryRepository.GetCategoryById(transaction.FromCategory)?.Type == "Баланс" || this.categoryRepository.GetCategoryById(transaction.FromCategory)?.Type == "Дохід") && this.categoryRepository.GetCategoryById(transaction.ToCategory)?.Type == "Витрати")
                    //{
                    //    transaction.Amount *= -1;
                    //}

                    data.Add(new TransactionDTO(transaction));
                }
            }

            this.dataGrid.ItemsSource = data;

            List<Transaction>? plannedData = this.transactionService.GetTransactions(App.GetCurrentUser())?.Where(c => (c.Date > DateTime.Now.Date && c.Planned == true)).ToList();
            List<TransactionDTO> rows = new List<TransactionDTO>();
            foreach (Transaction transaction in plannedData)
            {
                if (transaction != null)
                {
                    rows.Add(new TransactionDTO(transaction));
                }
            }

            this.dataGridPlannedTransactions.ItemsSource = rows;
        }

        private void CreateTransactionButton_Click(object sender, RoutedEventArgs e)
        {
            var all_fields_valid = this.AllFieldsValid();
            if (all_fields_valid)
            {
                string category_from_title = (string)this.sourceComboBox.SelectedValue;
                string category_to_title = (string)this.destinationComboBox.SelectedValue;
                decimal amount = decimal.Parse(this.amountTextBox.Text);
                string details = this.detailsTextBox.Text;
                bool planned = this.checkboxIsPlanned.IsChecked.HasValue && this.checkboxIsPlanned.IsChecked.Value;

                DateTime? date = default(DateTime);

                Guid? category_from = this.categoryRepository.GetCategoryByName(App.GetCurrentUser()?.Id, category_from_title)?.Id;
                Guid? category_to = this.categoryRepository.GetCategoryByName(App.GetCurrentUser()?.Id, category_to_title)?.Id;

                if (!planned)
                {
                    date = DateTime.Now;
                }
                else
                {
                    date = this.datePickerPlannedDate?.SelectedDate.Value;
                }

                //if ((this.categoryRepository.GetCategoryById(category_from)?.Type == "Баланс" || this.categoryRepository.GetCategoryById(category_from)?.Type == "Дохід") && this.categoryRepository.GetCategoryById(category_to)?.Type == "Витрати")
                //{
                //    amount *= -1;
                //}

                this.transactionService.AddTransaction(App.GetCurrentUser(), category_to, category_from, amount, date, details, planned);

                if (planned)
                {
                    List<Transaction>? plannedData = this.transactionService.GetTransactions(App.GetCurrentUser())?.Where(c => (c.Date > DateTime.Now.Date && c.Planned == true)).ToList();
                    List<TransactionDTO> rows = new List<TransactionDTO>();
                    foreach (Transaction transaction in plannedData)
                    {
                        if (transaction != null)
                        {
                            rows.Add(new TransactionDTO(transaction));
                        }
                    }

                    this.dataGridPlannedTransactions.ItemsSource = rows;
                }
                else
                {
                    List<Transaction>? defaultData = this.transactionService.GetTransactions(App.GetCurrentUser())?.Where(c => c.Date < DateTime.Now.Date || (c.Date?.Date == DateTime.Now.Date && c.Planned == false)).ToList();
                    List<TransactionDTO> data = new List<TransactionDTO>();
                    foreach (Transaction transaction in defaultData)
                    {
                        if (transaction != null)
                        {
                            data.Add(new TransactionDTO(transaction));
                        }
                    }

                    this.dataGrid.ItemsSource = data;
                }

                this.sourceComboBox.SelectedIndex = -1;
                this.destinationComboBox.SelectedIndex = -1;
                this.detailsTextBox.Text = string.Empty;
                this.amountTextBox.Text = string.Empty;
                this.datePickerPlannedDate.SelectedDate = null;
            }
        }

        private bool AllFieldsValid()
        {
            string error_mes = string.Empty;
            error_mes = this.SourceComboBox_Error();

            if (error_mes == string.Empty)
            {
                error_mes = this.DestinationComboBox_Error();
            }

            if (error_mes == string.Empty)
            {
                error_mes = this.AmountTextBox_Error();
            }

            if (error_mes == string.Empty)
            {
                error_mes = this.DatePickerPlannedDate_Error();
            }

            this.errormessage.Text = error_mes;
            return this.errormessage.Text == string.Empty;
        }

        private string SourceComboBox_Error()
        {
            string category_from_title = (string)this.sourceComboBox.SelectedValue;
            if (string.IsNullOrEmpty(category_from_title))
            {
                return "Оберіть джерело.";
            }
            else
            {
                return string.Empty;
            }
        }

        private string DestinationComboBox_Error()
        {
            string category_to_title = (string)this.destinationComboBox.SelectedValue;

            if (string.IsNullOrEmpty(category_to_title))
            {
                return "Оберіть призначення";
            }
            else
            {
                return string.Empty;
            }
        }

        private string AmountTextBox_Error()
        {
            if (string.IsNullOrWhiteSpace(this.amountTextBox.Text))
            {
                return "Введіть суму.";
            }
            else if (!decimal.TryParse(this.amountTextBox.Text, out decimal result))
            {
                return "Некоректний формат суми.";
            }
            else if (result < 0)
            {
                return "Сума повинна бути не менше 0.";
            }
            else
            {
                return string.Empty;
            }
        }

        private string DatePickerPlannedDate_Error()
        {
            if (this.checkboxIsPlanned.IsChecked.HasValue && this.checkboxIsPlanned.IsChecked.Value)
            {
                if (string.IsNullOrWhiteSpace(this.datePickerPlannedDate.Text))
                {
                    return "Оберіть дату.";
                }
                else if (!DateTime.TryParse(this.datePickerPlannedDate.Text, out DateTime selectedDate))
                {
                    return "Оберіть коректну дату";
                }
                else if (selectedDate.Date < DateTime.Today)
                {
                    return "Дата не може бути у минулому";
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.DatePickerBorder.Visibility = Visibility.Visible;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.DatePickerBorder.Visibility = Visibility.Collapsed;
        }

        private ScrollViewer? GetScrollViewer(DependencyObject? depObj)
        {
            if (depObj is ScrollViewer viewer)
            {
                return viewer;
            }

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                var result = this.GetScrollViewer(child);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        private void DataGridPlannedScrollBackward_Click(object sender, RoutedEventArgs e)
        {
            var scrollViewer = this.GetScrollViewer(this.dataGridPlannedTransactions);

            double jumpSize = Math.Floor(scrollViewer.ViewportHeight);

            scrollViewer.ScrollToVerticalOffset(Math.Max(0, scrollViewer.VerticalOffset - jumpSize));
        }

        private void DataGridPlannedScrollForward_Click(object sender, RoutedEventArgs e)
        {
            var scrollViewer = this.GetScrollViewer(this.dataGridPlannedTransactions);

            double jumpSize = Math.Floor(scrollViewer.ViewportHeight);

            scrollViewer.ScrollToVerticalOffset(Math.Min(scrollViewer.ScrollableHeight, scrollViewer.VerticalOffset + jumpSize));
        }

        private void DataGridPlannedTransactions_Loaded(object sender, RoutedEventArgs e)
        {
            ScrollViewer? scrollViewer = this.GetScrollViewer(this.dataGridPlannedTransactions);
            if (scrollViewer != null)
            {
                scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            }
        }

        private void DataGridGeneralScrollBackward_Click(object sender, RoutedEventArgs e)
        {
            var scrollViewer = this.GetScrollViewer(this.dataGrid);

            double jumpSize = Math.Floor(scrollViewer.ViewportHeight);

            scrollViewer.ScrollToVerticalOffset(Math.Max(0, scrollViewer.VerticalOffset - jumpSize));
        }

        private void DataGridGeneralScrollForward_Click(object sender, RoutedEventArgs e)
        {
            var scrollViewer = this.GetScrollViewer(this.dataGrid);

            double jumpSize = Math.Floor(scrollViewer.ViewportHeight);

            scrollViewer.ScrollToVerticalOffset(Math.Min(scrollViewer.ScrollableHeight, scrollViewer.VerticalOffset + jumpSize));
        }

        private void DataGridGeneralTransactions_Loaded(object sender, RoutedEventArgs e)
        {
            ScrollViewer? scrollViewer = this.GetScrollViewer(this.dataGrid);
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
    }
}
