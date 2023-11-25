using ThinkTwice_Context;
using BLL.DTO;

namespace BLL
{
    public class AuthenticationService
    {
        private readonly UserRepository _userService = new UserRepository();

        public UserDTO? AuthenticateUser(string email, string password)
        {
            var user = _userService.GetUserByEmail(email);
            if (user != null)
            {
                UserDTO userDTO = new UserDTO(user);
                if (PasswordHasher.VerifyPassword(user.Password, password))
                {
                    return userDTO;
                }
                else { return null; }
            }
            else { return null; }
        }
    }
    public class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyPassword(string hashedPassword, string candidatePassword)
        {
            return BCrypt.Net.BCrypt.Verify(candidatePassword, hashedPassword);
        }
    }
}
