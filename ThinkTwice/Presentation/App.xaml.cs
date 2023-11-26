using Bogus.DataSets;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ThinkTwice_Context;
using BLL.DTO;
using System.Windows.Navigation;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static UserDTO? currentUser { get; set; }

        public UserDTO GetCurrentUser()
        {
            return currentUser;
        }

        // Власний метод set для CurrentUser
        public static void SetCurrentUser(UserDTO user)
        {
            currentUser = user;
        }
        public static void RemoveUser()
        {
            currentUser = null;
        }
    }
}
