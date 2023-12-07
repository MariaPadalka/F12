namespace BLL
{
    using BLL.DTO;
    using ThinkTwice_Context;

    public class AuthenticationService
    {
        public UserRepository userService = new UserRepository();

        public UserDTO? AuthenticateUser(string email, string password)
        {
            var user = this.userService.GetUserByEmail(email);
            if (user != null)
            {
                UserDTO userDTO = new UserDTO(user);
                if (PasswordHasher.VerifyPassword(user.Password, password))
                {
                    return userDTO;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
