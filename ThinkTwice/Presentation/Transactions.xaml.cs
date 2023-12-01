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

namespace Presentation
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Transactions : Page
    {
        private readonly TransactionService _transactionService = new TransactionService();
        public Transactions()
        {
            InitializeComponent();
            Loaded += YourWindow_Loaded;
            //InitializeData();
        }

        private void YourWindow_Loaded(object sender, RoutedEventArgs e)
        {
            List<Transaction>? defaultData = _transactionService.GetTransactions(App.GetCurrentUser());
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

        //private void InitializeData()
        //{
        //    List<Transaction> defaultData = new List<Transaction>
        //{
        //    new Transaction {Details = "Salary", Date = DateTime.Now, Amount = 1000.00M, Planned = false },
        //    new Transaction {Details = "Groceries", Date = DateTime.Now, Amount = -50.00M, Planned = false },
        //    new Transaction {Details = "Scholarship", Date = DateTime.Now, Amount = 800.00M, Planned = false },
        //    new Transaction {Details = "Utilities", Date = DateTime.Now, Amount = -120.00M, Planned = false },
        //    new Transaction {Details = "Salary", Date = DateTime.Now, Amount = 1000.00M, Planned = false },
        //    new Transaction {Details = "Clothes", Date = DateTime.Now, Amount = -70.00M, Planned = false },
        //};

        //    dataGrid.ItemsSource = defaultData;
        //}


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
        public void Logout(object sender, RoutedEventArgs e)
        {
            App.RemoveUser();
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new Uri("Login.xaml", UriKind.Relative));
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text) || textBox.Text == "Джерело")
            {
                textBox.Text = string.Empty;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "Джерело";
            }
        }



        //private void PreviewMouseDownOutsideTextBox(object sender, MouseButtonEventArgs e)
        //{
        //    // Get the element that was clicked
        //    FrameworkElement clickedElement = e.OriginalSource as FrameworkElement;

        //    // Check if the clicked area is not within the TextBox
        //    if (!IsMouseOverTextBox(clickedElement))
        //    {
        //        TextBox textBox = FindTextBox(clickedElement); // Find the TextBox dynamically
        //        if (string.IsNullOrWhiteSpace(textBox.Text))
        //        {
        //            textBox.Text = "Your Text Here";
        //        }
        //    }
        //}

        //private bool IsMouseOverTextBox(FrameworkElement element)
        //{
        //    // Check if the element is a TextBox or is inside a TextBox
        //    return element is TextBox || VisualTreeHelper.GetParent(element) is TextBox;
        //}

        //private TextBox FindTextBox(FrameworkElement element)
        //{
        //    // Find the closest parent of the specified element that is a TextBox
        //    while (element != null && !(element is TextBox))
        //    {
        //        element = VisualTreeHelper.GetParent(element) as FrameworkElement;
        //    }

        //    return element as TextBox;
        //}


    }
}
