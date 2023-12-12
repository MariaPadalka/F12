namespace BLL
{
    using BLL.DTO;
    using ThinkTwice_Context;

    public class TransactionService
    {
        public TransactionRepository TransactionRepo = new TransactionRepository();
        public UserRepository UserRepo = new UserRepository();
        public CategoryRepository CategoryRepo = new CategoryRepository();

        public List<Transaction>? GetTransactions(UserDTO? userDTO)
        {
            if (userDTO != null)
            {
                var user = this.UserRepo.GetUserById(userDTO.Id);
                if (user != null)
                {
                    return this.TransactionRepo.GetTransactionsByUserId(user.Id);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public List<Transaction>? GetTransactionsInTimePeriod(UserDTO? userDTO, DateTime? start, DateTime? end)
        {
            if (userDTO != null)
            {
                var user = this.UserRepo.GetUserById(userDTO.Id);
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

                    return this.TransactionRepo.GetTransactionsByUserId(user.Id).Where(i => i.Date >= start && i.Date <= end).ToList();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public void AddTransaction(UserDTO? userDTO, Guid? category_to, Guid? category_from, decimal amount, DateTime? date, string? details, bool planned)
        {
            if (userDTO != null && category_from != null && category_to != null)
            {
                var user = this.UserRepo.GetUserById(userDTO.Id);
                var categoryTo = this.CategoryRepo.GetCategoryById(category_to);
                var categoryFrom = this.CategoryRepo.GetCategoryById(category_from);
                if (user != null && categoryTo != null && categoryFrom != null)
                {
                    var newTransaction = new Transaction
                    {
                        UserId = user.Id,
                        FromCategory = category_from,
                        ToCategory = category_to,
                        Amount = amount,
                        Date = date,
                        Details = details,
                        Planned = planned,
                    };
                    this.TransactionRepo.CreateTransaction(newTransaction);
                }
            }
        }

        public void UpdateTransaction(UserDTO? userDTO, Guid? transactionId, Guid? category_to, Guid? category_from, decimal amount, DateTime? date, string? details, bool planned)
        {
            if (userDTO != null && category_from != null && category_to != null && transactionId != null)
            {
                var user = this.UserRepo.GetUserById(userDTO.Id);
                var categoryTo = this.CategoryRepo.GetCategoryById(category_to);
                var categoryFrom = this.CategoryRepo.GetCategoryById(category_from);
                var transaction = this.TransactionRepo.GetTransactionById(transactionId);
                if (user != null && categoryTo != null && categoryFrom != null && transaction != null)
                {
                    if (transaction.UserId == user.Id)
                    {
                        transaction.FromCategory = category_from;
                        transaction.ToCategory = category_to;
                        transaction.Amount = amount;
                        transaction.Date = date;
                        transaction.Details = details;
                        transaction.Planned = planned;
                        this.TransactionRepo.Update(transaction);
                    }
                }
            }
        }

        public void RemoveTransaction(UserDTO? userDTO, Guid? transactionId)
        {
            if (userDTO != null && transactionId != null)
            {
                var user = this.UserRepo.GetUserById(userDTO.Id);
                if (user != null)
                {
                    this.TransactionRepo.Delete(transactionId);
                }
            }
        }

        public Transaction? GetTransaction(UserDTO? userDTO, Guid? transactionId)
        {
            if (userDTO != null && transactionId != null)
            {
                var user = this.UserRepo.GetUserById(userDTO.Id);
                var transaction = this.TransactionRepo.GetTransactionById(transactionId);
                if (user != null && transaction != null)
                {
                    if (transaction.UserId == user.Id)
                    {
                        return transaction;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public decimal GetExpenses(UserDTO? userDTO)
        {
            if (userDTO != null)
            {
                DateTime start = DateTime.Now.AddDays(-30);
                var transactions = this.GetTransactionsInTimePeriod(userDTO, start, null);
                if (transactions != null)
                {
                    return transactions.Where(i => this.CategoryRepo.GetCategoryById(i.ToCategory)?.Type == "Витрати").Sum(i => i.Amount);
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        public decimal GetIncome(UserDTO? userDTO)
        {
            if (userDTO != null)
            {
                DateTime start = DateTime.Now.AddDays(-30);
                var transactions = this.GetTransactionsInTimePeriod(userDTO, start, null);
                if (transactions != null)
                {
                    return transactions.Where(i => this.CategoryRepo.GetCategoryById(i.FromCategory)?.Type == "Дохід" &&
                     this.CategoryRepo.GetCategoryById(i.ToCategory)?.Type == "Баланс"
                    && this.CategoryRepo.GetCategoryById(i.ToCategory)?.Title != "Скарбничка").Sum(i => i.Amount);
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        public decimal GetBalance(UserDTO? userDTO)
        {
            if (userDTO != null)
            {
                var transactions = this.GetTransactionsInTimePeriod(userDTO, null, null);
                if (transactions != null)
                {
                    var income = transactions.Where(i => this.CategoryRepo.GetCategoryById(i.FromCategory)?.Type == "Дохід" &&
                     this.CategoryRepo.GetCategoryById(i.ToCategory)?.Type == "Баланс"
                    && this.CategoryRepo.GetCategoryById(i.ToCategory)?.Title != "Скарбничка").Sum(i => i.Amount);
                    var expenses = transactions.Where(i => this.CategoryRepo.GetCategoryById(i.ToCategory)?.Type == "Витрати").Sum(i => i.Amount);
                    var savings = this.GetSavings(userDTO);
                    return income - (expenses + savings);
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        public decimal GetSavings(UserDTO? userDTO)
        {
            if (userDTO != null)
            {
                var transactions = this.GetTransactionsInTimePeriod(userDTO, null, null);
                if (transactions != null)
                {
                    var inc = transactions.Where(i => this.CategoryRepo.GetCategoryById(i.ToCategory)?.Title == "Скарбничка").Sum(i => i.Amount);
                    var exp = transactions.Where(i => this.CategoryRepo.GetCategoryById(i.FromCategory)?.Title == "Скарбничка").Sum(i => i.Amount);
                    return inc - exp;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
    }
}
