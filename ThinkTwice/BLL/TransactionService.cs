using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkTwice_Context;
using BLL.DTO;

namespace BLL
{
    public class TransactionService
    {
        private readonly TransactionRepository _transactionRepository = new TransactionRepository();
        private readonly UserRepository _userService = new UserRepository();
        private readonly CategoryRepository _categoryRepository = new CategoryRepository();

        public List<Transaction>? GetTransactions(UserDTO? userDTO)
        {
            if (userDTO != null)
            {
                var user = _userService.GetUserById(userDTO.Id);
                if (user != null)
                {
                    return _transactionRepository.GetTransactionsByUserId(user.Id);
                }
                else { return null; }
            }
            else { return null; }
        }
        public List<Transaction>? GetTransactionsInTimePeriod(UserDTO? userDTO, DateTime? start, DateTime? end)
        {
            if (userDTO != null)
            {
                var user = _userService.GetUserById(userDTO.Id);
                if (user != null)
                {
                    if (end == null)
                    {
                        end = DateTime.Now;
                    }
                    if (start == null)
                    {
                        start = new DateTime(2000, 1, 1);
                    }
                    return _transactionRepository.GetTransactionsByUserId(user.Id).Where(i => i.Date >= start && i.Date <= end).ToList();
                }
                else { return null; }
            }
            else { return null; }
        }
        public void AddTransaction(UserDTO? userDTO, Guid? category_to, Guid? category_from, decimal Amount, DateTime? Date, string Title, string? Details, bool Planned)
        {
            if (userDTO != null && category_from != null && category_to != null)
            {
                var user = _userService.GetUserById(userDTO.Id);
                var categoryTo = _categoryRepository.GetCategoryById(category_to);
                var categoryFrom = _categoryRepository.GetCategoryById(category_from);
                if (user != null && categoryTo != null && categoryFrom != null)
                {
                    var newTransaction = new Transaction
                    {
                        UserId = user.Id,
                        FromCategory = category_from,
                        ToCategory = category_to,
                        Amount = Amount,
                        Date = Date,
                        Title = Title,
                        Details = Details,
                        Planned = Planned
                    };
                    _transactionRepository.CreateTransaction(newTransaction);
                }
            }
        }
        public void UpdateTransaction(UserDTO? userDTO, Guid? transactionId, Guid? category_to, Guid? category_from, decimal Amount, DateTime? Date, string Title, string? Details, bool Planned)
        {
            if (userDTO != null && category_from != null && category_to != null && transactionId != null)
            {
                var user = _userService.GetUserById(userDTO.Id);
                var categoryTo = _categoryRepository.GetCategoryById(category_to);
                var categoryFrom = _categoryRepository.GetCategoryById(category_from);
                var transaction = _transactionRepository.GetTransactionById(transactionId);
                if (user != null && categoryTo != null && categoryFrom != null && transaction != null)
                {
                    if (transaction.UserId == user.Id)
                    {
                        transaction.FromCategory = category_from;
                        transaction.ToCategory = category_to;
                        transaction.Amount = Amount;
                        transaction.Date = Date;
                        transaction.Title = Title;
                        transaction.Details = Details;
                        transaction.Planned = Planned;
                        _transactionRepository.Update(transaction);
                    }
                }
            }
        }
        public void RemoveTransaction(UserDTO? userDTO, Guid? transactionId)
        {
            if (userDTO != null && transactionId != null)
            {
                var user = _userService.GetUserById(userDTO.Id);
                if (user != null)
                {
                    _transactionRepository.Delete(transactionId);
                }
            }
        }
        public Transaction? GetTransaction(UserDTO? userDTO, Guid? transactionId)
        {
            if (userDTO != null && transactionId != null)
            {
                var user = _userService.GetUserById(userDTO.Id);
                var transaction = _transactionRepository.GetTransactionById(transactionId);
                if (user != null && transaction != null)
                {
                    if (transaction.UserId == user.Id)
                    {
                        return transaction;
                    } else { return null; }
                } else { return null; }
            } else { return null; }
        }
        public decimal GetExpenses(UserDTO? userDTO)
        {
            if (userDTO != null)
            {
                DateTime start = DateTime.Now.AddDays(-740);
                var transactions = GetTransactionsInTimePeriod(userDTO, start, null);
                if (transactions != null)
                {
                    return transactions.Where(i => _categoryRepository.GetCategoryById(i.ToCategory)?.Type == "Витрати").Sum(i => i.Amount);
                }
                else { return 0; }
            } else { return 0; }
        }
        public decimal GetIncome(UserDTO? userDTO)
        {
            if (userDTO != null)
            {
                DateTime start = DateTime.Now.AddDays(-740);
                var transactions = GetTransactionsInTimePeriod(userDTO, start, null);
                if (transactions != null)
                {
                    return transactions.Where(i => _categoryRepository.GetCategoryById(i.ToCategory)?.Type == "Дохід").Sum(i => i.Amount);
                }
                else { return 0; }
            } else { return 0; }
        }
        public decimal GetBalance(UserDTO? userDTO)
        {
            if (userDTO != null)
            {
                var transactions = GetTransactionsInTimePeriod(userDTO, null, null);
                if (transactions != null)
                {
                    return transactions.Where(i => _categoryRepository.GetCategoryById(i.ToCategory)?.Type == "Баланс").Sum(i => i.Amount);
                } else { return 0; }
            } else { return 0; }
        }
    }
}
