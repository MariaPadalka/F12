// <copyright file="Dashboard.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Presentation
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Navigation;
    using BLL;
    using LiveCharts;
    using LiveCharts.Wpf;
    using Presentation.DTO;
    using Serilog;
    using ThinkTwice_Context;

    /// <summary>
    /// Interaction logic for Dashboard.xaml.
    /// </summary>
    public partial class Dashboard : Page
    {
        private readonly ILogger logger = LoggerManager.Instance.Logger;

        private readonly TransactionService transactionService = new TransactionService();
        private readonly CategoryRepository categoryRepository = new CategoryRepository();

        /// <summary>
        /// Initializes a new instance of the <see cref="Dashboard"/> class.
        /// </summary>
        public Dashboard()
        {
            this.InitializeComponent();
            this.Loaded += this.YourWindow_Loaded;
            this.Loaded += this.PaintGraphic;

            this.logger.Information("Перехід на головну панель.");
        }

        public SeriesCollection? SeriesCollection { get; set; }

        public string[]? Labels { get; set; }

        public Func<double, string>? Formatter { get; set; }

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

        public void Statistics_Click(object sender, RoutedEventArgs e)
        {
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new Uri("Statistics.xaml", UriKind.Relative));
        }

        public void Logout(object sender, RoutedEventArgs e)
        {
            this.logger.Information($"Вихід користувача {App.CurrentUser.Name} {App.CurrentUser.Surname}.");
            App.RemoveUser();
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new Uri("Login.xaml", UriKind.Relative));
        }

        private void YourWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var currency = this.GetCurrency(App.GetCurrentUser()?.Currency);
            List<Transaction>? defaultData = this.transactionService.GetTransactions(App.GetCurrentUser())?.Where(i => i.Planned == false).ToList();
            List<TransactionDTO>? transactionDTOs = new List<TransactionDTO>();
            foreach (var transaction in defaultData)
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

                    transactionDTOs.Add(new TransactionDTO(transaction_copy));
                }
            }

            var incomes = this.transactionService.GetIncome(App.GetCurrentUser());
            var expense = this.transactionService.GetExpenses(App.GetCurrentUser());
            var balance = this.transactionService.GetBalance(App.GetCurrentUser());
            var savings = this.transactionService.GetSavings(App.GetCurrentUser());
            this.incomeValue.Text = currency + incomes.ToString();
            this.balanceValue.Text = currency + balance.ToString();
            this.expensesValue.Text = currency + expense.ToString();
            this.savingsValue.Text = currency + savings.ToString();
            this.dataGrid.ItemsSource = transactionDTOs.OrderBy(i => i.Date).Reverse();
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
            var transactions = this.transactionService.GetTransactionsInTimePeriod(App.GetCurrentUser(), DateTime.Now.AddDays(-7), DateTime.Now);
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
                for (DateTime? date_ = date.Item1.AddDays(1); date_ <= date.Item2; date_ = date_?.AddDays(1))
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
                    new LineSeries
                    {
                        Title = "Доходи",
                        Values = incomes,
                        PointGeometry = null,
                        Fill = new SolidColorBrush(System.Windows.Media.Colors.Transparent),
                        Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0x90, 0xD7, 0xB9)),
                        StrokeThickness = 2,
                    },
                };

                this.SeriesCollection.Add(new LineSeries
                {
                    Title = "Збереженя",
                    Values = savings,
                    PointGeometry = null,
                    Fill = new SolidColorBrush(System.Windows.Media.Colors.Transparent),
                    Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0xBD, 0xD4, 0xF1)),
                    StrokeThickness = 2,
                });

                this.SeriesCollection.Add(new LineSeries
                {
                    Title = "Витрати",
                    Values = expenses,
                    PointGeometry = null,
                    Fill = new SolidColorBrush(System.Windows.Media.Colors.Transparent),
                    Stroke = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0xFF, 0xB6, 0xAE)),
                    StrokeThickness = 2,
                });

                int currentDayOfWeek = (int)DateTime.Now.DayOfWeek;
                CultureInfo ukrainianCulture = new CultureInfo("uk-UA");
                var daysNum = datesInRange.Select(i => i?.ToString("dd MMMM"));
                this.Labels = new string[7];
                for (int i = 0; i < 7; i++)
                {
                    this.Labels[i] = daysNum.ElementAt(i);
                }

                this.Formatter = value => value.ToString("N");
            }

            this.x.DataContext = this;
        }
    }
}