namespace ThinkTwice_Context
{
    using Microsoft.EntityFrameworkCore;

    public class TransactionRepository
    {
        private readonly ThinkTwiceContext context = new ThinkTwiceContext();

        public virtual void CreateTransaction(Transaction transaction)
        {
            this.context.Transactions.Add(transaction);
            this.context.SaveChanges();
        }

        public virtual Transaction? GetTransactionById(Guid? transactionId)
        {
            return this.context.Transactions.FirstOrDefault(c => c.Id == transactionId);
        }

        public virtual List<Transaction> GetTransactionsByUserId(Guid userId)
        {
            return this.context.Transactions.Where(c => c.UserId == userId).ToList();
        }

        public List<Transaction> GetTransactionsByFromCategoryId(Guid userId, Guid fromCategoryId)
        {
            return this.context.Transactions.Where(c => c.UserId == userId && c.FromCategory == fromCategoryId).ToList();
        }

        public List<Transaction> GetTransactionsByToCategoryId(Guid userId, Guid toCategoryId)
        {
            return this.context.Transactions.Where(c => c.UserId == userId && c.FromCategory == toCategoryId).ToList();
        }

        public List<Transaction> GetPlannedTransactions(Guid? userId)
        {
            return this.context.Transactions.Where(c => c.UserId == userId && c.Planned).ToList();
        }

        public virtual void Update(Transaction cat)
        {
            this.context.Entry(cat).State = EntityState.Modified;
            this.context.SaveChanges();
        }

        public virtual void Delete(Guid? id)
        {
            var category = this.context.Transactions.Find(id);
            if (category != null)
            {
                this.context.Transactions.Remove(category);
                this.context.SaveChanges();
            }
        }
    }
}
