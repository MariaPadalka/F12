namespace BLL
{
    using BLL.DTO;
    using ThinkTwice_Context;

    public class AuthenticationService
    {
        public UserRepository UserRepo = new UserRepository();

        public UserDTO? AuthenticateUser(string email, string password)
        {
            var user = this.UserRepo.GetUserByEmail(email);
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
