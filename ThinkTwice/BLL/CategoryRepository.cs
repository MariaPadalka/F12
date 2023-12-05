namespace BLL
{
    using Microsoft.EntityFrameworkCore;
    using ThinkTwice_Context;

    public class CategoryRepository
    {
        private readonly ThinkTwiceContext context = new ThinkTwiceContext();

        public void CreateCategory(string title, bool isGeneral, decimal percentageAmount, string type, Guid? userId)
        {
            var newCategory = new Category
            {
                Title = title,
                IsGeneral = isGeneral,
                PercentageAmount = percentageAmount,
                Type = type,
                UserId = userId,
            };
            this.context.Categories.Add(newCategory);
            this.context.SaveChanges();
        }

        public Category? GetCategoryById(Guid? categoryId)
        {
            return this.context.Categories.FirstOrDefault(c => c.Id == categoryId);
        }

        public List<Category> GetCategoriesByUserId(Guid? userId)
        {
            return this.context.Categories.Where(c => c.UserId == userId).ToList();
        }

        public List<Category> GetGeneralCategories()
        {
            return this.context.Categories.Where(c => c.IsGeneral == true).ToList();
        }

        public Category? GetCategoryByName(Guid? userId, string name)
        {
            return this.context.Categories.FirstOrDefault(c => (c.UserId == userId || c.IsGeneral == true) && c.Title == name);
        }

        public List<Category> GetCategoriesByType(Guid? userId, string type)
        {
            return this.context.Categories.Where(c => c.UserId == userId && c.Type == type).ToList();
        }

        public void Update(Category cat)
        {
            this.context.Entry(cat).State = EntityState.Modified;
            this.context.SaveChanges();
        }

        public void Delete(Guid? id)
        {
            var category = this.context.Categories.Find(id);
            if (category != null)
            {
                var transactionsWithCategory = this.context.Transactions
                    .Where(t => t.FromCategory == category.Id || t.ToCategory == category.Id);

                foreach (var transaction in transactionsWithCategory)
                {
                    if (transaction.FromCategory == category.Id)
                    {
                        transaction.FromCategory = null;
                    }

                    if (transaction.ToCategory == category.Id)
                    {
                        transaction.ToCategory = null;
                    }
                }

                this.context.Categories.Remove(category);

                this.context.SaveChanges();
            }
        }
    }
}
