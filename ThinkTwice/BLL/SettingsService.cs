using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.DTO;
using ThinkTwice_Context;

namespace BLL
{
    public class SettingsService
    {
        private readonly UserRepository _userService = new UserRepository();
        private readonly CategoryRepository _categoryService = new CategoryRepository();

        public UserDTO? UpdateUser(UserDTO updatedUser)
        {
            if (updatedUser != null)
            {
                var user = _userService.GetUserById(updatedUser.Id);
                if (user != null)
                {
                    user.Email = updatedUser.Email;
                    user.Surname = updatedUser.Surname;
                    user.Name = updatedUser.Name;
                    user.BirthDate = updatedUser.BirthDate;
                    user.Currency = updatedUser.Currency;
                    _userService.Update(user);
                    return updatedUser;
                }
                else { return null; }
            }
            else { return null; }
        }
        public UserDTO? UpdatePassword(UserDTO userDTO, string password)
        {
            if (userDTO != null)
            {
                var user = _userService.GetUserById(userDTO.Id);
                if (user != null)
                {
                    string hashedPassword = PasswordHasher.HashPassword(password);
                    user.Password = hashedPassword;
                    _userService.Update(user);
                    return userDTO;
                }
                else { return null; }
            } else { return null; }
        }
        public List<Category>? GetUserCategories(UserDTO userDTO)
        {
            if (userDTO != null)
            {
                var user = _userService.GetUserById(userDTO.Id);
                if (user != null)
                {
                    return _categoryService.GetCategoriesByUserId(user.Id);
                } else { return null; }
            } else { return null; }
        }
        public void RemoveCategory(UserDTO userDTO, Guid categoryId)
        {
            if (userDTO != null)
            {
                var user = _userService.GetUserById(userDTO.Id);
                if (user != null)
                {
                    var category = _categoryService.GetCategoryById(categoryId);
                    if (category != null)
                    {
                        if (category.UserId == userDTO.Id)
                        {
                            _categoryService.Delete(categoryId);
                        }
                    }
                }
            }
        }
    }
}
