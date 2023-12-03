using BLL.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkTwice_Context;

namespace BLL
{
    public class CategoryService
    {
        private readonly TransactionRepository _transactionRepository = new TransactionRepository();
        private readonly UserRepository _userRepository = new UserRepository();
        public readonly CategoryRepository _categoryRepository = new CategoryRepository();


        public List<string>? GetCategoriesTitleByType(UserDTO? userDTO, string type)
        {
            if (userDTO != null)
            {
                var user = _userRepository.GetUserById(userDTO.Id);
                if (user != null)
                {
                    List<string>? userCategories = _categoryRepository.GetCategoriesByType(user.Id, type).Where(c => c.Type == type).Select(c => c.Title).ToList();
                    List<string>? generalCategories = _categoryRepository.GetGeneralCategories().Where(c => c.Type == type).Select(c => c.Title).ToList();
                    List<string> allCategories = userCategories.Concat(generalCategories).ToList();
                    return allCategories;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return _categoryRepository.GetGeneralCategories().Select(c => c.Title).ToList();
            }
        }
    }
}
