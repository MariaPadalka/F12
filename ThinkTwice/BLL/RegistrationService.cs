using System;
using System.Collections.Generic;
using System.Linq;
using ThinkTwice_Context;

namespace BLL
{
    public class RegistrationService
    {
        private readonly ThinkTwiceContext _context = new ThinkTwiceContext();
        public void Register(string email, string password, string first_name, string last_name, DateTime? date, string currency)
        {
            string hashedPassword = PasswordHasher.HashPassword(password);

            var newUser = new User
            {
                Email = email,
                Password = hashedPassword,
                Name = first_name,
                Surname = last_name,
                BirthDate = (DateTime)date,
                Currency = currency
            };
            _context.Users.Add(newUser);
            _context.SaveChanges();
        }
    }
}
