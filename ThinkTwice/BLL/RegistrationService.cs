namespace BLL
{
    using BLL.DTO;
    using ThinkTwice_Context;

    public class RegistrationService
    {
        public UserRepository UserRepo = new UserRepository();

        public UserDTO? Register(string email, string password, string first_name, string last_name, DateTime? date, string currency)
        {
            var user = this.UserRepo.GetUserByEmail(email);
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
                    Currency = currency,
                };
                this.UserRepo.Add(newUser);
                UserDTO userDTO = new UserDTO(newUser);
                return userDTO;
            }

            return null;
        }
    }
}
