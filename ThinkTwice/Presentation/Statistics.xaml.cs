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
    using BLL.DTO;
    using LiveCharts;
    using LiveCharts.Wpf;
    using Serilog;
    using ThinkTwice_Context;

    /// <summary>
    /// Interaction logic for Statistics.xaml.
    /// </summary>
    /// 

    public partial class Statistics : Page
    {
        private readonly ILogger logger = LoggerManager.Instance.Logger;

        private static (DateTime?, DateTime?) statDate;
        private readonly TransactionService transactionService = new TransactionService();
        private readonly CategoryRepository categoryRepository = new CategoryRepository();

        //LiveCharts.Wpf.PieChart pieChartData = new LiveCharts.Wpf.PieChart();


        /// <summary>
        ///// Initializes a new instance of the <see cref="Statistics"/> class.
        /// </summary>
        public Statistics()
        {
            this.InitializeComponent();
            SimplePieControl();
            PlannedPie();

            this.Loaded += this.YourWindow_Loaded;

            this.logger.Information("Перехід на сторінку статистики.");
        }

        public SeriesCollection? SeriesCollection { get; set; }

        public string[]? Labels { get; set; }

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
            //this.PopulateChartData(App.GetCurrentUser());
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
                var savings = new ChartValues<decimal>(new decimal[7]);
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
                    decimal totalSavings = 0;
                    var group = groupedTransactions.FirstOrDefault(i => i.Date?.DayOfYear == date_?.DayOfYear);
                    if (group != null)
                    {
                        foreach (var transaction in group.Transactions)
                        {
                            var fromCategory = this.categoryRepository.GetCategoryById(transaction.FromCategory);
                            var toCategory = this.categoryRepository.GetCategoryById(transaction.ToCategory);

                            if (toCategory != null)
                            {
                                if (toCategory.Type == "Витрати")
                                {
                                    totalExpenses += transaction.Amount;
                                }
                                else if (fromCategory.Type == "Дохід" && toCategory.Type == "Баланс" && toCategory.Title != "Скарбничка")
                                {
                                    totalIncomes += transaction.Amount;
                                }
                                else if (toCategory.Title == "Скарбничка")
                                {
                                    totalSavings += transaction.Amount;
                                }

                                if (fromCategory.Title == "Скарбничка")
                                {
                                    totalSavings -= transaction.Amount;
                                }
                            }
                        }
                    }

                    expenses[j] = totalExpenses;
                    incomes[j] = totalIncomes;
                    savings[j] = totalSavings;
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
                    Title = "Збереження",
                    Values = savings,
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

        private Dictionary<string, decimal>? GetCategoryExpensesInLastMonth(UserDTO userDTO)
        {
            DateTime startDate = DateTime.Now.AddMonths(-1); // Початок останнього місяця
            DateTime endDate = DateTime.Now;

            var transactions = this.transactionService.GetTransactionsInTimePeriod(userDTO, startDate, endDate).Where(x => x.Planned == false && x.ToCategory!= null);

            var transactionsWithCategoryName = transactions?
            .Select(t => new
            {
                Id = t.Id,
                Date = t.Date,
                Amount = t.Amount,
                CategoryName = this.categoryRepository.GetCategoryById(t.ToCategory).Title,
                Type = this.categoryRepository.GetCategoryById(t.ToCategory).Type,
                isGeneral = this.categoryRepository.GetCategoryById(t.ToCategory).IsGeneral
            })
            .ToList();

            if (transactionsWithCategoryName != null)
            {
                var categoryExpenses = transactionsWithCategoryName
                    .Where(t => t.Type == "Витрати" && t.isGeneral == false) // Враховуємо тільки витрати (суму менше 0)
                    .GroupBy(t => t.CategoryName) // Групуємо транзакції за категоріями
                    .ToDictionary(g => g.Key, g => g.Sum(t => t.Amount)); // Сумуємо витрати для кожної категорії

                return categoryExpenses;
            }

            return null;
        }

        private Dictionary<string, decimal>? GetCategoryPlannedExpenses(UserDTO userDTO)
        {
            var categories = this.categoryRepository.GetCategoriesByUserId(userDTO.Id).Where(c => c.Type == "Витрати");

            if (categories != null)
            {
                var dict = categories.ToDictionary(g => g.Title, g => g.PercentageAmount); // Сумуємо витрати для кожної категорії

                return dict;
            }

            return null;
        }
        public void PlannedPie()
        {
            Func<ChartPoint, string> labelPoint = chartPoint =>
                string.Format("{0:P}", chartPoint.Participation);

            Dictionary<string, decimal>? categoryExpenses = this.GetCategoryPlannedExpenses(App.GetCurrentUser());
            decimal sum_of_percents = categoryExpenses.Values.Sum();

            if (categoryExpenses != null)
            {
                this.plannedPieChart.Series = new SeriesCollection();

                foreach (var t in categoryExpenses)
                {
                    var pieSeries = new PieSeries
                    {
                        Title = t.Key,
                        Values = new ChartValues<decimal> { t.Value },
                        DataLabels = true,
                        LabelPoint = labelPoint,
                    };

                    this.plannedPieChart.Series.Add(pieSeries);
                }

                if (sum_of_percents < 100)
                {
                    var remainingSeries = new PieSeries
                    {
                        Title = "Залишок",
                        Values = new ChartValues<decimal> { 100 - sum_of_percents },
                        DataLabels = true,
                        LabelPoint = labelPoint,
                    };

                    this.plannedPieChart.Series.Add(remainingSeries);
                }
            }

            // Налаштування легенди
            this.plannedPieChart.LegendLocation = LegendLocation.Bottom;
        }



        public void SimplePieControl()
        {
            Func<ChartPoint, string> labelPoint = chartPoint =>
                string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);

            Dictionary<string, decimal>? categoryExpenses = this.GetCategoryExpensesInLastMonth(App.GetCurrentUser());
            if (categoryExpenses != null)
            {
                this.myPieChart.Series = new SeriesCollection();
                foreach(var t in categoryExpenses)
                {
                    this.myPieChart.Series.Add(new PieSeries { 
                        Title = t.Key,
                        Values = new ChartValues<decimal> { t.Value },
                        DataLabels = true,
                        LabelPoint = labelPoint
                    });
                }
            }

            myPieChart.LegendLocation = LegendLocation.Bottom;
        }
    }
}
