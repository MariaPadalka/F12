namespace Presentation
{
    using System.Windows;
    using BLL.DTO;

    /// <summary>
    /// Interaction logic for App.xaml.
    /// </summary>
    public partial class App : Application
    {
        public static UserDTO? CurrentUser { get; set; }

        public static UserDTO? GetCurrentUser()
        {
            return CurrentUser;
        }

        public static void SetCurrentUser(UserDTO user)
        {
            CurrentUser = user;
        }

        public static void RemoveUser()
        {
            CurrentUser = null;
        }
    }
}
