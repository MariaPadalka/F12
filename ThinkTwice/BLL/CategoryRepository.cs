using ThinkTwice_Context;
using Microsoft.EntityFrameworkCore;

namespace BLL
{
    public class CategoryRepository
    {
        private readonly ThinkTwiceContext _context = new ThinkTwiceContext();

        public void CreateCategory(string Title, bool IsGeneral, decimal PercentageAmount, string Type, Guid? userId)
        {
            var newCategory = new Category
            {
                Title = Title,
                IsGeneral = IsGeneral,
                PercentageAmount = PercentageAmount,
                Type = Type,
                UserId = userId
            };
            _context.Categories.Add(newCategory);
            _context.SaveChanges();
        }
        public Category GetCategoryById(Guid? categoryId)
        {
            return _context.Categories.FirstOrDefault(c => c.Id == categoryId);
        }
        public List<Category> GetCategoriesByUserId(Guid? userId)
        {
            return _context.Categories.Where(c => c.UserId == userId).ToList();
        }
        public List<Category> GetGeneralCategories()
        {
            return _context.Categories.Where(c => c.IsGeneral == true).ToList();
        }
        public Category GetCategoryByName(Guid? userId, string name)
        {
            return _context.Categories.FirstOrDefault(c => (c.UserId == userId || c.IsGeneral == true) && c.Title == name);
        } 
        public List<Category> GetCategoriesByType(Guid? userId, string type)
        {
            return _context.Categories.Where(c => c.UserId == userId && c.Type == type).ToList();
        }
        public void Update(Category cat)
        {
            _context.Entry(cat).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(Guid? id)
        {
            var category = _context.Categories.Find(id);
            if (category != null)
            {
                // Знайдемо всі транзакції, що містять цю категорію
                var transactionsWithCategory = _context.Transactions
                    .Where(t => t.FromCategory == category.Id || t.ToCategory == category.Id);

                // Проходимося по всіх транзакціях і встановлюємо значення в null
                foreach (var transaction in transactionsWithCategory)
                {
                    if (transaction.FromCategory == category.Id)
                        transaction.FromCategory = null;

                    if (transaction.ToCategory == category.Id)
                        transaction.ToCategory = null;
                }

                // Видаляємо категорію
                _context.Categories.Remove(category);

                // Зберігаємо зміни в базі даних
                _context.SaveChanges();
            }
        }
    }
}
