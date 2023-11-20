﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using BLL;

namespace Presentation
{
    public partial class Login : Page
    {
        UserRepository user_repo = new UserRepository();

        public Login()
        {
            InitializeComponent();
        }
        private void Registration_Click(object sender, RoutedEventArgs e)
        {
            //RegistrationView reg = new RegistrationView();
            //contentContainer.Content = reg;
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new Uri("RegistrationView.xaml", UriKind.Relative));
        }
        private void GoToDashboard(object sender, RoutedEventArgs e)
        {
            //Dashboard dashboard = new Dashboard();
            //contentContainer.Content = dashboard;
            NavigationService ns = NavigationService.GetNavigationService(this);
            ns.Navigate(new Uri("Dashboard.xaml", UriKind.Relative));
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            string email = textBoxEmail.Text;
            string password = passwordBox1.Password;
            AuthenticationService authenticationService = new AuthenticationService(user_repo);

            if (!IsEmailValid(email))
            {
                errormessage.Text = "Введіть коректну електронну пошту.";
            }
            else 
            {
                if (authenticationService.AuthenticateUser(email, password) == null)
                {
                    errormessage.Text = "Електронної пошти не знайдено.";
                }
                else if (authenticationService.AuthenticateUser(email, password) == false)
                {
                    errormessage.Text = "Неправильний пароль";
                }
                else
                {
                    errormessage.Text = "";
                    GoToDashboard(sender, e);
                }
            }
        }
        public static bool IsEmailValid(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            string emailPattern = @"^[a-zA-Z0-9._%+-]{3,20}@[a-zA-Z0-9.-]{2,20}\.[a-zA-Z]{2,10}$";

            return Regex.IsMatch(email, emailPattern);
        }
    }

}
