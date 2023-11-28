﻿using System;
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
            /*InitializeData();*/
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

        //    private void InitializeData()
        //    {
        //        List<Transaction> defaultData = new List<Transaction>
        //{
        //    new Transaction { Title = "Income", Details = "Salary", Date = DateTime.Now, Amount = 1000.00M, Planned = false },
        //    new Transaction { Title = "Expense", Details = "Groceries", Date = DateTime.Now, Amount = -50.00M, Planned = false },
        //    new Transaction { Title = "Income", Details = "Scholarship", Date = DateTime.Now, Amount = 800.00M, Planned = false },
        //    new Transaction { Title = "Expense", Details = "Utilities", Date = DateTime.Now, Amount = -120.00M, Planned = false },
        //    new Transaction { Title = "Income", Details = "Salary", Date = DateTime.Now, Amount = 1000.00M, Planned = false },
        //    new Transaction { Title = "Expense", Details = "Clothes", Date = DateTime.Now, Amount = -70.00M, Planned = false },
        //};

        //        transactionsListView.ItemsSource = defaultData;
        //    }


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
    }
}
