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
    using LiveCharts;
    using LiveCharts.Wpf;

    /// <summary>
    /// Interaction logic for Statistics.xaml.
    /// </summary>
    public partial class Statistics : Page
    {
        private static (DateTime?, DateTime?) statDate;
        private readonly TransactionService transactionService = new TransactionService();
        private readonly CategoryRepository categoryRepository = new CategoryRepository();

        /// <summary>
        /// Initializes a new instance of the <see cref="Statistics"/> class.
        /// </summary>
        public Statistics()
        {
            this.InitializeComponent();
            this.Loaded += this.YourWindow_Loaded;
        }

        public SeriesCollection? SeriesCollection { get; set; }

        public string[] Labels { get; set; }

        public Func<double, string>? Formatter { get; set; }

        public void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new Uri("Dashboard.xaml", UriKind.Relative));
        }

        public void TransactionsClick(object sender, RoutedEventArgs e)
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

        private void YourWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.RefreshChartData();
        }

        private void RefreshChartData()
        {
            if (statDate.Item1 == null)
            {
                statDate = (DateTime.Now.AddDays(-7), null);
            }

            this.GetTransactionGraphic(statDate);
            this.x.Update(true, true);
        }

        private void GetTransactionGraphic((DateTime?, DateTime?) date)
        {
            var transactions = this.transactionService.GetTransactionsInTimePeriod(App.GetCurrentUser(), date.Item1, date.Item2);
            if (transactions != null)
            {
                var groupedTransactions = transactions
                    .GroupBy(t => t.Date)
                    .Select(group => new
                    {
                        Date = group.Key,
                        Transactions = group.ToList(),
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
                            var category = this.categoryRepository.GetCategoryById(transaction.ToCategory);

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

                this.SeriesCollection = new SeriesCollection
                {
                    new ColumnSeries
                    {
                        Title = "Доходи",
                        Values = incomes,
                        ColumnPadding = 6,
                        Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x90, 0xD7, 0xB9)),
                    },
                };

                this.SeriesCollection.Add(new ColumnSeries
                {
                    Title = "Баланс",
                    Values = balance,
                    ColumnPadding = 6,
                    Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0xBD, 0xD4, 0xF1)),
                });

                this.SeriesCollection.Add(new ColumnSeries
                {
                    Title = "Витрати",
                    Values = expenses,
                    ColumnPadding = 6,
                    Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0xFF, 0xB6, 0xAE)),
                });

                int currentDayOfWeek = (int)DateTime.Now.DayOfWeek;

                var daysNum = datesInRange.Select(i => i?.ToString("dd.MM"));
                string[] days = new[] { "Понеділок", "Вівторок", "Середа", "Четвер", "П'ятниця", "Субота", "Неділя" };
                this.Labels = new string[7];
                for (int i = 0; i < 7; i++)
                {
                    this.Labels[i] = days[(currentDayOfWeek + i) % 7] + "\n" + daysNum.ElementAt(i);
                }

                this.Formatter = value => value.ToString("N");
            }

            this.x.DataContext = this;
            this.x.Series = this.SeriesCollection;
            var axisX = this.x.AxisX.FirstOrDefault();
            if (axisX != null)
            {
                axisX.Labels = this.Labels;
            }
        }

        private void PreviousWeek(object sender, MouseButtonEventArgs e)
        {
            statDate = (statDate.Item1?.AddDays(-7), statDate.Item1);
            this.RefreshChartData();
        }

        private void NextWeek(object sender, MouseButtonEventArgs e)
        {
            statDate = (statDate.Item2, statDate.Item2?.AddDays(7));
            this.RefreshChartData();
        }
    }
}
