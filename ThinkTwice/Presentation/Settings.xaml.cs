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

namespace Presentation
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Settings : Page
    {
        public Settings()
        {
            InitializeComponent();
            Loaded += YourWindow_Loaded;
            /*InitializeData();*/


        }

        private void YourWindow_Loaded(object sender, RoutedEventArgs e)
        {
            List<Transaction> defaultData = new List<Transaction>
            {
                new Transaction { Title = "Income", Details = "Salary", Date = DateTime.Now.Date, Amount = 1000.00M, Planned = false },
                new Transaction { Title = "Expense", Details = "Groceries", Date = DateTime.Now.Date, Amount = -50.00M, Planned = false },
                new Transaction { Title = "Income", Details = "Scholarship", Date = DateTime.Now.Date, Amount = 800.00M, Planned = false },
                new Transaction { Title = "Expense", Details = "Utilities", Date = DateTime.Now.Date, Amount = -120.00M, Planned = false },
                new Transaction { Title = "Income", Details = "Salary", Date = DateTime.Now.Date, Amount = 1000.00M, Planned = false },
                //new Transaction { Title = "Expense", Details = "Clothes", Date = DateTime.Now.Date, Amount = -70.00M, Planned = false },
            };

        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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

    }
}
