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
using LiveCharts;
using LiveCharts.Wpf;
using System.Globalization;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Page
    {
        private readonly TransactionService _transactionService = new TransactionService();
        private readonly CategoryRepository _categoryRepository = new CategoryRepository();
        public Dashboard()
        {
            InitializeComponent();
            Loaded += YourWindow_Loaded;
            Loaded += PaintGraphic;
            /*InitializeData();*/
        }

        private void YourWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var currency = GetCurrency(App.GetCurrentUser()?.Currency);
            List<Transaction>? defaultData = _transactionService.GetTransactions(App.GetCurrentUser()).Where(i => i.Planned == false).ToList();
            var incomes = _transactionService.GetIncome(App.GetCurrentUser());
            var expense = _transactionService.GetExpenses(App.GetCurrentUser());
            incomeValue.Text = currency + incomes.ToString();
            balanceValue.Text = currency + _transactionService.GetBalance(App.GetCurrentUser()).ToString();
            expensesValue.Text = currency + expense.ToString();
            savingsValue.Text = currency + (incomes - expense).ToString();
            dataGrid.ItemsSource = defaultData;
        }

        public void Transactions_Click(object sender, RoutedEventArgs e)
        {
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new Uri("Transactions.xaml", UriKind.Relative));
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

        private string GetCurrency(string? currency)
        {
            switch (currency)
            {
                case "USD": return "$";
                case "EUR": return "€";
                case "UAH": return "₴";
                default: return "₴";
            }
        }

        private void PaintGraphic(object sender, RoutedEventArgs e)
        {
            var date = (DateTime.Now.AddDays(-7), DateTime.Now);
            var transactions = _transactionService.GetTransactionsInTimePeriod(App.GetCurrentUser(), DateTime.Now.AddDays(-7), DateTime.Now);
            if (transactions != null)
            {
                var groupedTransactions = transactions
                    .GroupBy(t => t.Date)
                    .Select(group => new
                    {
                        Date = group.Key,
                        Transactions = group.ToList()
                    }).Reverse()
                    .ToList();
                var expenses = new ChartValues<decimal>(new decimal[7]);
                var incomes = new ChartValues<decimal>(new decimal[7]);
                var balance = new ChartValues<decimal>(new decimal[7]);
                var j = 0;
                List<DateTime?> datesInRange = new List<DateTime?>();
                if (date.Item2 == null)
                {
                    date.Item2 = DateTime.Now;
                }

                for (DateTime? date_ = date.Item1.AddDays(1); date_ <= date.Item2; date_ = date_?.AddDays(1))
                {
                    datesInRange.Add(date_);
                }
                foreach (var date_ in datesInRange)
                {
                    decimal totalExpenses = 0;
                    decimal totalIncomes = 0;
                    decimal totalBalance = 0;
                    var group = groupedTransactions.FirstOrDefault(i => i.Date?.DayOfYear == date_?.DayOfYear);
                    if (group != null)
                    {
                        foreach (var transaction in group.Transactions)
                        {
                            var category = _categoryRepository.GetCategoryById(transaction.ToCategory);

                            if (category != null)
                            {
                                if (category.Type == "Витрати")
                                {
                                    totalExpenses += transaction.Amount;
                                }
                                else if (category.Type == "Дохід")
                                {
                                    totalIncomes += transaction.Amount;
                                }
                                else if (category.Type == "Баланс")
                                {
                                    totalBalance += transaction.Amount;
                                }
                            }

                        }
                    }
                    expenses[j] = totalExpenses;
                    incomes[j] = totalIncomes;
                    balance[j] = totalBalance;
                    j += 1;
                }

                SeriesCollection = new SeriesCollection
                {
                    new LineSeries
                    {
                        Title = "Доходи",
                        Values = incomes,
                        PointGeometry = null,
                        Fill = new SolidColorBrush(System.Windows.Media.Colors.Transparent), // Set fill color to transparent
                        Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x90, 0xD7, 0xB9)),
                        StrokeThickness = 2
                    }
                };

                SeriesCollection.Add(new LineSeries
                {
                    Title = "Баланс",
                    Values = balance,
                    PointGeometry = null,
                    Fill = new SolidColorBrush(System.Windows.Media.Colors.Transparent), // Set fill color to transparent
                    Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0xBD, 0xD4, 0xF1)),
                    StrokeThickness = 2
                });

                SeriesCollection.Add(new LineSeries
                {
                    Title = "Витрати",
                    Values = expenses,
                    PointGeometry = null,
                    Fill = new SolidColorBrush(System.Windows.Media.Colors.Transparent), // Set fill color to transparent
                    Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0xFF, 0xB6, 0xAE)),
                    StrokeThickness = 2
                });


                int currentDayOfWeek = (int)DateTime.Now.DayOfWeek;
             
                
                CultureInfo ukrainianCulture = new CultureInfo("uk-UA");
                var daysNum = datesInRange.Select(i => i?.ToString("dd MMMM"));
                Labels = new string[7];
                for (int i = 0; i < 7; i++)
                {
                    Labels[i] = daysNum.ElementAt(i);
                }
                Formatter = value => value.ToString("N");
            }
            x.DataContext = this;
        }

        public SeriesCollection? SeriesCollection { get; set; }

        public string[]? Labels { get; set; }

        public Func<double, string>? Formatter { get; set; }
    }
}