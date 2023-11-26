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

namespace Presentation
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Page
    {
        private readonly TransactionService _transactionService = new TransactionService();
        public Dashboard()
        {
            InitializeComponent();
            Loaded += YourWindow_Loaded;
            /*InitializeData();*/


        }

        private void YourWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var currency = GetCurrency(App.GetCurrentUser()?.Currency);
            List<Transaction>? defaultData = _transactionService.GetTransactions(App.GetCurrentUser());
            var incomes = _transactionService.GetIncome(App.GetCurrentUser());
            var expense = _transactionService.GetExpenses(App.GetCurrentUser());
            incomeValue.Text = currency + incomes.ToString();
            balanceValue.Text = currency + _transactionService.GetBalance(App.GetCurrentUser()).ToString();
            expensesValue.Text = currency + expense.ToString();
            savingsValue.Text = currency + (incomes - expense).ToString();
            dataGrid.ItemsSource = defaultData;
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public void Transactions_Click(object sender, RoutedEventArgs e)
        {
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new Uri("Transactions.xaml", UriKind.Relative));
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

    }
}
