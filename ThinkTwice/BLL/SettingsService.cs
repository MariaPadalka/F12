namespace BLL
{
    using BLL.DTO;
    using ThinkTwice_Context;

    public class SettingsService
    {
        public UserRepository UserRepo = new UserRepository();
        public CategoryRepository CategoryRepo = new CategoryRepository();

        public UserDTO? UpdateUser(UserDTO? updatedUser)
        {
            if (updatedUser != null)
            {
                var user = this.UserRepo.GetUserById(updatedUser.Id);
                if (user != null)
                {
                    user.Email = updatedUser.Email;
                    user.Surname = updatedUser.Surname;
                    user.Name = updatedUser.Name;
                    user.BirthDate = updatedUser.BirthDate;
                    user.Currency = updatedUser.Currency;
                    user.Categories = updatedUser.Categories;
                    this.UserRepo.Update(user);
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

        public IEnumerable<Category>? GetUserCategories(UserDTO? userDTO)
        {
            if (userDTO != null)
            {
                var user = this.UserRepo.GetUserById(userDTO.Id);
                if (user != null)
                {
                    List<Category>? userCategories = this.CategoryRepo.GetCategoriesByUserId(user.Id);
                    List<Category>? generalCategories = this.CategoryRepo.GetGeneralCategories();
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
                return this.CategoryRepo.GetGeneralCategories();
            }
        }

        public void RemoveCategory(UserDTO? userDTO, Guid categoryId)
        {
            if (userDTO != null)
            {
                var user = this.UserRepo.GetUserById(userDTO.Id);
                if (user != null)
                {
                    var category = this.CategoryRepo.GetCategoryById(categoryId);
                    if (category != null)
                    {
                        if (category.UserId == userDTO.Id)
                        {
                            this.CategoryRepo.Delete(categoryId);
                        }
                    }
                }
            }
        }

        public bool UniqueEmail(string? email)
        {
            if (email != null)
            {
                var user = this.UserRepo.GetUserByEmail(email);
                return user == null;
            }

            return false;
        }
    }
}
