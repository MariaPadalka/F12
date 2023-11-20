using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class AuthenticationService
    {
        private readonly UserRepository _userService;

        public AuthenticationService(UserRepository userService)
        {
            _userService = userService;
        }

        public bool? AuthenticateUser(string email, string password)
        {
            var user = _userService.GetUserByEmail(email);

            if (user != null)
            {
                if (PasswordHasher.VerifyPassword(user.Password, password))
                {
                    return true;
                }
                else { return false; }
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
