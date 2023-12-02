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
using LiveCharts;
using LiveCharts.Wpf;
using BLL;
using System.ComponentModel;


namespace Presentation
{
    /// <summary>
    /// Interaction logic for Statistics.xaml
    /// </summary>
    public partial class Statistics : Page
    {
        private readonly TransactionService _transactionService = new TransactionService();
        private readonly CategoryRepository _categoryRepository = new CategoryRepository();
        private static (DateTime?, DateTime?) _date;

        public Statistics()
        {
            InitializeComponent();
            Loaded += YourWindow_Loaded;
        }

        private void YourWindow_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshChartData();
        }

        private void RefreshChartData()
        {
            if (_date.Item1 == null)
            {
                _date = (DateTime.Now.AddDays(-7), null);
            }
            GetTransactionGraphic(_date);
            x.Update(true, true); 
        }

        public SeriesCollection? SeriesCollection { get; set; }
        public string[]? Labels { get; set; }

        private void GetTransactionGraphic((DateTime?, DateTime?) date)
        { 
            var transactions = _transactionService.GetTransactionsInTimePeriod(App.GetCurrentUser(), date.Item1, date.Item2);
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

                for (DateTime? date_ = date.Item1?.AddDays(1); date_ <= date.Item2; date_ = date_?.AddDays(1))
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
                    new ColumnSeries
                    {
                        Title = "Доходи",
                        Values = incomes,
                        ColumnPadding = 6,
                        Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x90, 0xD7, 0xB9))
                    }
                };

                SeriesCollection.Add(new ColumnSeries
                {
                    Title = "Баланс",
                    Values = balance,
                    ColumnPadding = 6,
                    Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0xBD, 0xD4, 0xF1))
                });

                SeriesCollection.Add(new ColumnSeries
                {
                    Title = "Витрати",
                    Values = expenses,
                    ColumnPadding = 6,
                    Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0xFF, 0xB6, 0xAE))
                });
               
                int currentDayOfWeek = (int)DateTime.Now.DayOfWeek;

                var daysNum = datesInRange.Select(i => i?.ToString("dd.MM"));
                string[] days = new[] { "Понеділок", "Вівторок", "Середа", "Четвер", "П'ятниця", "Субота", "Неділя" };
                Labels = new string[7];
                for (int i = 0; i < 7; i++)
                {
                    Labels[i] = days[(currentDayOfWeek + i) % 7] +"\n" + daysNum.ElementAt(i);
                }
                Formatter = value => value.ToString("N");
            }
            x.DataContext = this;
            x.Series = SeriesCollection;
            var axisX = x.AxisX.FirstOrDefault();
            if (axisX != null)
            {
                axisX.Labels = Labels;
            }
        }

        public Func<double, string>? Formatter { get; set; }

        public void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new Uri("Dashboard.xaml", UriKind.Relative));
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

        public void Logout(object sender, RoutedEventArgs e)
        {
            App.RemoveUser();
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new Uri("Login.xaml", UriKind.Relative));
        }

        private void PreviousWeek(object sender, MouseButtonEventArgs e)
        {
            _date = (_date.Item1?.AddDays(-7), _date.Item1);
            RefreshChartData();
        }
        
        private void NextWeek(object sender, MouseButtonEventArgs e)
        {
            _date = (_date.Item2, _date.Item2?.AddDays(7));
            RefreshChartData();
        }
    }
}
