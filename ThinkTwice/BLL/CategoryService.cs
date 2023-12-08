namespace BLL
{
    using BLL.DTO;
    using ThinkTwice_Context;

    public class CategoryService
    {
        public CategoryRepository CategoryRepo = new CategoryRepository();
        public UserRepository UserRepo = new UserRepository();

        public List<string>? GetCategoriesTitleByType(UserDTO? userDTO, string type)
        {
            if (userDTO != null)
            {
                var user = this.UserRepo.GetUserById(userDTO.Id);
                if (user != null)
                {
                    List<string>? userCategories = this.CategoryRepo.GetCategoriesByType(user.Id, type).Where(c => c.Type == type).Select(c => c.Title).ToList();
                    List<string>? generalCategories = this.CategoryRepo.GetGeneralCategories().Where(c => c.Type == type).Select(c => c.Title).ToList();
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
                return this.CategoryRepo.GetGeneralCategories().Where(c => c.Type == type).Select(c => c.Title).ToList();
            }
        }
    }
}
