namespace BLL
{
    using BLL.DTO;
    using ThinkTwice_Context;

    public class SettingsService
    {
        private readonly UserRepository userService = new UserRepository();
        private readonly CategoryRepository categoryService = new CategoryRepository();

        public UserDTO? UpdateUser(UserDTO updatedUser)
        {
            if (updatedUser != null)
            {
                var user = this.userService.GetUserById(updatedUser.Id);
                if (user != null)
                {
                    user.Email = updatedUser.Email;
                    user.Surname = updatedUser.Surname;
                    user.Name = updatedUser.Name;
                    user.BirthDate = updatedUser.BirthDate;
                    user.Currency = updatedUser.Currency;
                    user.Categories = updatedUser.Categories;
                    this.userService.Update(user);
                    return updatedUser;
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

        public UserDTO? UpdatePassword(UserDTO userDTO, string password)
        {
            if (userDTO != null)
            {
                var user = this.userService.GetUserById(userDTO.Id);
                if (user != null)
                {
                    string hashedPassword = PasswordHasher.HashPassword(password);
                    user.Password = hashedPassword;
                    this.userService.Update(user);
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

        public IEnumerable<Category>? GetUserCategories(UserDTO? userDTO)
        {
            if (userDTO != null)
            {
                var user = this.userService.GetUserById(userDTO.Id);
                if (user != null)
                {
                    List<Category>? userCategories = this.categoryService.GetCategoriesByUserId(user.Id);
                    List<Category>? generalCategories = this.categoryService.GetGeneralCategories();
                    IEnumerable<Category>? allCategories = userCategories.Concat(generalCategories).ToList();
                    return allCategories;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return this.categoryService.GetGeneralCategories();
            }
        }

        public void RemoveCategory(UserDTO userDTO, Guid categoryId)
        {
            if (userDTO != null)
            {
                var user = this.userService.GetUserById(userDTO.Id);
                if (user != null)
                {
                    var category = this.categoryService.GetCategoryById(categoryId);
                    if (category != null)
                    {
                        if (category.UserId == userDTO.Id)
                        {
                            this.categoryService.Delete(categoryId);
                        }
                    }
                }
            }
        }

        public bool UniqueEmail(string email)
        {
            if (email != null)
            {
                var user = this.userService.GetUserByEmail(email);
                return user == null;
            }

            return false;
        }
    }
}
