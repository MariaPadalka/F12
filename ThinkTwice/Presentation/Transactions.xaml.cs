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
    using Serilog;
    using ThinkTwice_Context;

    /// <summary>
    /// Interaction logic for Dashboard.xaml.
    /// </summary>
    /// 

    public partial class Transactions : Page
    {
        private readonly ILogger logger = LoggerManager.Instance.Logger;

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

            this.ResetFiels();
            this.DatePickerBorder.Visibility = Visibility.Collapsed;

            this.logger.Information("Перехід на сторінку транзакції.");
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

            if (combinedList.Count > 0)
            {
                this.sourceComboBox.SelectedIndex = 0;
            }
        }

        public void ComboBox_Destination()
        {
            List<string>? arr1 = this.categoryService.GetCategoriesTitleByType(App.GetCurrentUser(), "Баланс");
            List<string>? arr2 = this.categoryService.GetCategoriesTitleByType(App.GetCurrentUser(), "Витрати");

            List<string>? combinedList = new List<string>(arr1);
            combinedList.AddRange(arr2);

            this.destinationComboBox.ItemsSource = combinedList;

            if (combinedList.Count > 0)
            {
                this.destinationComboBox.SelectedIndex = combinedList.Count - 1;
            }
        }

        private void YourWindow_Loaded(object sender, RoutedEventArgs e)
        {
            List<Transaction>? defaultData = this.transactionService.GetTransactions(App.GetCurrentUser())?.Where(c => c.Date < DateTime.Now.Date || (c.Date == DateTime.Now.Date && c.Planned == false)).ToList();
            List<TransactionDTO> data = new List<TransactionDTO>();
            foreach (Transaction transaction in defaultData)
            {
                var transaction_copy = new Transaction();
                transaction_copy.Date = transaction.Date;
                transaction_copy.UserId = transaction.UserId;
                transaction_copy.Amount = transaction.Amount;
                transaction_copy.FromCategory = transaction.FromCategory;
                transaction_copy.ToCategory = transaction.ToCategory;
                transaction_copy.Id = transaction.Id;
                transaction_copy.Details = transaction.Details;
                if (transaction_copy != null)
                {
                    if ((this.categoryRepository.GetCategoryById(transaction_copy.FromCategory)?.Type == "Баланс" || this.categoryRepository.GetCategoryById(transaction_copy.FromCategory)?.Type == "Дохід") && this.categoryRepository.GetCategoryById(transaction_copy.ToCategory)?.Type == "Витрати")
                    {
                        transaction_copy.Amount *= -1;
                    }

                    data.Add(new TransactionDTO(transaction_copy));
                }
            }

            this.dataGrid.ItemsSource = data.OrderBy(i => i.Date).Reverse();

            List<Transaction>? plannedData = this.transactionService.GetTransactions(App.GetCurrentUser())?.Where(c => (c.Date > DateTime.Now.Date && c.Planned == true)).ToList();
            List<TransactionDTO> rows = new List<TransactionDTO>();
            foreach (Transaction transaction in plannedData)
            {
                var transaction_copy = new Transaction();
                transaction_copy.Date = transaction.Date;
                transaction_copy.UserId = transaction.UserId;
                transaction_copy.Amount = transaction.Amount;
                transaction_copy.FromCategory = transaction.FromCategory;
                transaction_copy.ToCategory = transaction.ToCategory;
                transaction_copy.Id = transaction.Id;
                transaction_copy.Details = transaction.Details;
                if (transaction_copy != null)
                {
                    if ((this.categoryRepository.GetCategoryById(transaction_copy.FromCategory)?.Type == "Баланс" || this.categoryRepository.GetCategoryById(transaction_copy.FromCategory)?.Type == "Дохід") && this.categoryRepository.GetCategoryById(transaction_copy.ToCategory)?.Type == "Витрати")
                    {
                        transaction_copy.Amount *= -1;
                    }

                    rows.Add(new TransactionDTO(transaction_copy));
                }
            }

            this.dataGridPlannedTransactions.ItemsSource = rows.OrderBy(i => i.Date);
        }

        private void CreateTransactionButton_Click(object sender, RoutedEventArgs e)
        {
            this.logger.Information("Спроба додавання транзакції.");

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

                this.transactionService.AddTransaction(App.GetCurrentUser(), category_to, category_from, amount, date, details, planned);

                if (planned)
                {
                    List<Transaction>? plannedData = this.transactionService.GetTransactions(App.GetCurrentUser())?.Where(c => (c.Date > DateTime.Now.Date && c.Planned == true)).ToList();
                    List<TransactionDTO> rows = new List<TransactionDTO>();
                    foreach (Transaction transaction in plannedData)
                    {
                        var transaction_copy = new Transaction();
                        transaction_copy.Date = transaction.Date;
                        transaction_copy.UserId = transaction.UserId;
                        transaction_copy.Amount = transaction.Amount;
                        transaction_copy.FromCategory = transaction.FromCategory;
                        transaction_copy.ToCategory = transaction.ToCategory;
                        transaction_copy.Id = transaction.Id;
                        transaction_copy.Details = transaction.Details;
                        if (transaction_copy != null)
                        {
                            if ((this.categoryRepository.GetCategoryById(transaction_copy.FromCategory)?.Type == "Баланс" || this.categoryRepository.GetCategoryById(transaction_copy.FromCategory)?.Type == "Дохід") && this.categoryRepository.GetCategoryById(transaction_copy.ToCategory)?.Type == "Витрати")
                            {
                                transaction_copy.Amount *= -1;
                            }

                            rows.Add(new TransactionDTO(transaction_copy));
                        }
                    }

                    this.dataGridPlannedTransactions.ItemsSource = rows;

                    this.logger.Information("Додано заплановану транзакцію.");
                }
                else
                {
                    List<Transaction>? defaultData = this.transactionService.GetTransactions(App.GetCurrentUser())?.Where(c => c.Date < DateTime.Now.Date || (c.Date?.Date == DateTime.Now.Date && c.Planned == false)).ToList();
                    List<TransactionDTO> data = new List<TransactionDTO>();
                    foreach (Transaction transaction in defaultData)
                    {
                        var transaction_copy = new Transaction();
                        transaction_copy.Date = transaction.Date;
                        transaction_copy.UserId = transaction.UserId;
                        transaction_copy.Amount = transaction.Amount;
                        transaction_copy.FromCategory = transaction.FromCategory;
                        transaction_copy.ToCategory = transaction.ToCategory;
                        transaction_copy.Id = transaction.Id;
                        transaction_copy.Details = transaction.Details;
                        if (transaction_copy != null)
                        {
                            if ((this.categoryRepository.GetCategoryById(transaction_copy.FromCategory)?.Type == "Баланс" || this.categoryRepository.GetCategoryById(transaction_copy.FromCategory)?.Type == "Дохід") && this.categoryRepository.GetCategoryById(transaction_copy.ToCategory)?.Type == "Витрати")
                            {
                                transaction_copy.Amount *= -1;
                            }

                            data.Add(new TransactionDTO(transaction_copy));
                        }
                    }

                    this.dataGrid.ItemsSource = data;

                    this.logger.Information("Додано поточну транзакцію.");
                }

                this.ResetFiels();
            }
            else
            {
                this.logger.Error("Помилка додавання транзакції");
            }
        }

        private void ResetFiels()
        {
            this.ComboBox_Source();
            this.ComboBox_Destination();
            this.detailsTextBox.Text = "Деталі";
            this.amountTextBox.Text = "0,00";
            this.amountTextBox.Foreground = Brushes.Gray;
            this.detailsTextBox.Foreground = Brushes.Gray;
            this.datePickerPlannedDate.SelectedDate = DateTime.Now.AddDays(1).Date;
        }

        private void TextBoxDetails_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (textBox.Text == "Деталі")
            {
                textBox.Text = string.Empty;
                textBox.Foreground = Brushes.Black;
            }
        }

        private void TextBoxDetails_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Деталі";
                textBox.Foreground = Brushes.Gray;
            }
        }
        private void TextBoxAmount_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (textBox.Text == "0,00")
            {
                textBox.Text = string.Empty;
                textBox.Foreground = Brushes.Black;
            }
        }

        private void TextBoxAmount_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "0,00";
                textBox.Foreground = Brushes.Gray;
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
    }
}
