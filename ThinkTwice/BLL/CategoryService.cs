namespace BLL
{
    using BLL.DTO;
    using ThinkTwice_Context;

    public class CategoryService
    {
        public CategoryRepository categoryRepository = new CategoryRepository();
        public UserRepository userRepository = new UserRepository();

        public List<string>? GetCategoriesTitleByType(UserDTO? userDTO, string type)
        {
            if (userDTO != null)
            {
                var user = this.userRepository.GetUserById(userDTO.Id);
                if (user != null)
                {
                    List<string>? userCategories = this.categoryRepository.GetCategoriesByType(user.Id, type).Where(c => c.Type == type).Select(c => c.Title).ToList();
                    List<string>? generalCategories = this.categoryRepository.GetGeneralCategories().Where(c => c.Type == type).Select(c => c.Title).ToList();
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
                return this.categoryRepository.GetGeneralCategories().Where(c => c.Type == type).Select(c => c.Title).ToList();
            }
        }
    }
}
