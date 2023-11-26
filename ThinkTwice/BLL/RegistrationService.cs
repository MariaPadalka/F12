using System;
using System.Collections.Generic;
using System.Linq;
using ThinkTwice_Context;
using BLL.DTO;

namespace BLL
{
    public class RegistrationService
    {
        private readonly UserRepository _userService = new UserRepository();

        public UserDTO? Register(string email, string password, string first_name, string last_name, DateTime? date, string currency)
        {
            var user = _userService.GetUserByEmail(email);
            if (user == null)
            {
                string hashedPassword = PasswordHasher.HashPassword(password);

                var newUser = new User
                {
                    Email = email,
                    Password = hashedPassword,
                    Name = first_name,
                    Surname = last_name,
                    BirthDate = date,
                    Currency = currency
                };
                _userService.Add(newUser);
                UserDTO userDTO = new UserDTO(newUser);
                return userDTO;
            }
            return null;
        }
    }
}
