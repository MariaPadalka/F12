namespace BLL
{
    using BLL.DTO;

    public class CategoryService
    {
        private readonly CategoryRepository categoryRepository = new CategoryRepository();
        private readonly TransactionRepository transactionRepository = new TransactionRepository();
        private readonly UserRepository userRepository = new UserRepository();

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
                return this.categoryRepository.GetGeneralCategories().Select(c => c.Title).ToList();
            }
        }
    }
}
