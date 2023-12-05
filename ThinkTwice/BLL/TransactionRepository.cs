using Microsoft.EntityFrameworkCore;
using ThinkTwice_Context;

namespace BLL
{
    public class TransactionRepository
    {
        private readonly ThinkTwiceContext _context = new ThinkTwiceContext();

        public void CreateTransaction(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            _context.SaveChanges();
        }
        public Transaction? GetTransactionById(Guid? transactionId)
        {
            return _context.Transactions.FirstOrDefault(c => c.Id == transactionId);
        }
        public List<Transaction> GetTransactionsByUserId(Guid userId)
        {
            return _context.Transactions.Where(c => c.UserId == userId).ToList();
        }
        public List<Transaction> GetTransactionsByFromCategoryId(Guid userId, Guid fromCategoryId)
        {
            return _context.Transactions.Where(c => c.UserId == userId && c.FromCategory == fromCategoryId).ToList();
        }
        public List<Transaction> GetTransactionsByToCategoryId(Guid userId, Guid toCategoryId)
        {
            return _context.Transactions.Where(c => c.UserId == userId && c.FromCategory == toCategoryId).ToList();
        }
        public List<Transaction> GetExpensesForUser(Guid? userId)
        {
            return _context.Transactions.Where(c => c.UserId == userId && c.Amount < 0).ToList();
        }
        public List<Transaction> GetIncomesForUser(Guid? userId)
        {
            return _context.Transactions.Where(c => c.UserId == userId && c.Amount > 0).ToList();
        }

        public List<Transaction> GetPlannedTransactions(Guid? userId)
        {
            return _context.Transactions.Where(c => c.UserId == userId && c.Planned).ToList();
        }

        public void Update(Transaction cat)
        {
            _context.Entry(cat).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(Guid? id)
        {
            var category = _context.Transactions.Find(id);
            if (category != null)
            {
                _context.Transactions.Remove(category);
                _context.SaveChanges();
            }
        }
    }
}
