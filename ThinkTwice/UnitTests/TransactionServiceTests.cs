using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThinkTwice_Context;
using BLL;
using BLL.DTO;
using Moq;
using System.Collections.Generic;
using System;

namespace UnitTests
{
    [TestClass]
    public class TransactionServiceTests
    {
        [TestMethod]
        public void GetTransactions_WithValidUser_ReturnsTransactions()
        {
            // Arrange
            var userDTO = new UserDTO(new User { Id = Guid.NewGuid() });

            var userRepositoryMock = new Mock<UserRepository>();
            var transactionRepositoryMock = new Mock<TransactionRepository>();

            var user = new User { Id = Guid.NewGuid(), Email = "test@example.com" };
            userRepositoryMock.Setup(repo => repo.GetUserById(userDTO.Id)).Returns(user);

            var transactions = new List<Transaction>
            {
                new Transaction { Id = Guid.NewGuid(), UserId = user.Id, Amount = 100, Date = System.DateTime.Now, Planned = false },
                new Transaction { Id = Guid.NewGuid(), UserId = user.Id, Amount = 200, Date = System.DateTime.Now.AddDays(-1), Planned = true }
            };

            transactionRepositoryMock.Setup(repo => repo.GetTransactionsByUserId(user.Id)).Returns(transactions);

            var transactionService = new TransactionService
            {
                UserRepo = userRepositoryMock.Object,
                TransactionRepo = transactionRepositoryMock.Object
            };

            // Act
            var result = transactionService.GetTransactions(userDTO);

            // Assert
            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(transactions, result);
        }

        [TestMethod]
        public void GetTransactions_WithNotNullUserDTO_UserNotFound_ReturnsNull()
        {
            // Arrange
            var userDTO = new UserDTO(new User { Id = Guid.NewGuid() });

            var userRepositoryMock = new Mock<UserRepository>();
            var transactionRepositoryMock = new Mock<TransactionRepository>();

            userRepositoryMock.Setup(repo => repo.GetUserById(userDTO.Id)).Returns((User?)null);

            var transactionService = new TransactionService
            {
                UserRepo = userRepositoryMock.Object,
                TransactionRepo = transactionRepositoryMock.Object
            };

            // Act
            var result = transactionService.GetTransactions(userDTO);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetTransactions_WithNullUser_ReturnsNull()
        {
            // Arrange
            UserDTO? userDTO = null;

            var transactionService = new TransactionService();

            // Act
            var result = transactionService.GetTransactions(userDTO);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetTransactionsInTimePeriod_WithValidUser_ReturnsTransactionsInPeriod()
        {
            // Arrange
            var userDTO = new UserDTO(new User { Id = Guid.NewGuid() });
            var start = DateTime.Now.AddMonths(-1);
            var end = DateTime.Now;

            var userRepositoryMock = new Mock<UserRepository>();
            var transactionRepositoryMock = new Mock<TransactionRepository>();

            var user = new User { Id = userDTO.Id, Email = "test@example.com" };
            var transactions = new List<Transaction>
            {
                new Transaction { Id = Guid.NewGuid(), UserId = user.Id, Amount = 100, Date = start.AddDays(1) },
                new Transaction { Id = Guid.NewGuid(), UserId = user.Id, Amount = 200, Date = end.AddDays(-1) },
                new Transaction { Id = Guid.NewGuid(), UserId = user.Id, Amount = 300, Date = end.AddDays(1) },
                new Transaction { Id = Guid.NewGuid(), UserId = user.Id, Amount = 400, Date = start.AddDays(-1) }
            };

            userRepositoryMock.Setup(repo => repo.GetUserById(userDTO.Id)).Returns(user);
            transactionRepositoryMock.Setup(repo => repo.GetTransactionsByUserId(user.Id)).Returns(transactions);

            var transactionService = new TransactionService
            {
                UserRepo = userRepositoryMock.Object,
                TransactionRepo = transactionRepositoryMock.Object
            };

            // Act
            var result = transactionService.GetTransactionsInTimePeriod(userDTO, start, end);

            // Assert
            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(transactions.GetRange(0, 2), result);
        }

        [TestMethod]
        public void GetTransactionsInTimePeriod_WithNullUserDTO_ReturnsNull()
        {
            // Arrange
            UserDTO? userDTO = null;
            var start = DateTime.Now.AddMonths(-1);
            var end = DateTime.Now;

            var transactionService = new TransactionService();

            // Act
            var result = transactionService.GetTransactionsInTimePeriod(userDTO, start, end);

            // Assert
            Assert.IsNull(result);
        }
        
        [TestMethod]
        public void GetTransactionsInTimePeriod_WithNotNullUserDTO_UserNotFound_ReturnsNull()
        {
            // Arrange
            var userDTO = new UserDTO(new User { Id = Guid.NewGuid() });

            var userRepositoryMock = new Mock<UserRepository>();
            var transactionRepositoryMock = new Mock<TransactionRepository>();

            userRepositoryMock.Setup(repo => repo.GetUserById(userDTO.Id)).Returns((User?)null);

            var start = DateTime.Now.AddMonths(-1);
            var end = DateTime.Now;

            var transactionService = new TransactionService
            {
                UserRepo = userRepositoryMock.Object,
                TransactionRepo = transactionRepositoryMock.Object
            };
            // Act
            var result = transactionService.GetTransactionsInTimePeriod(userDTO, start, end);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetTransactionsInTimePeriod_WithValidUserAndNullStart_ReturnsTransactionsInPeriod()
        {
            // Arrange
            var userDTO = new UserDTO(new User { Id = Guid.NewGuid() });
            DateTime? start = null;
            var end = DateTime.Now;

            var userRepositoryMock = new Mock<UserRepository>();
            var transactionRepositoryMock = new Mock<TransactionRepository>();

            var user = new User { Id = userDTO.Id, Email = "test@example.com" };
            var transactions = new List<Transaction>
            {
                new Transaction { Id = Guid.NewGuid(), UserId = user.Id, Amount = 100, Date = end.AddDays(-2) },
                new Transaction { Id = Guid.NewGuid(), UserId = user.Id, Amount = 200, Date = end.AddDays(-1) },
                new Transaction { Id = Guid.NewGuid(), UserId = user.Id, Amount = 300, Date = end.AddDays(1) }
            };

            userRepositoryMock.Setup(repo => repo.GetUserById(userDTO.Id)).Returns(user);
            transactionRepositoryMock.Setup(repo => repo.GetTransactionsByUserId(user.Id)).Returns(transactions);

            var transactionService = new TransactionService
            {
                UserRepo = userRepositoryMock.Object,
                TransactionRepo = transactionRepositoryMock.Object
            };

            // Act
            var result = transactionService.GetTransactionsInTimePeriod(userDTO, start, end);

            // Assert
            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(transactions.GetRange(0, 2), result);
        }

        [TestMethod]
        public void GetTransactionsInTimePeriod_WithValidUserAndNullEnd_ReturnsTransactionsInPeriod()
        {
            // Arrange
            var userDTO = new UserDTO(new User { Id = Guid.NewGuid() });
            var start = DateTime.Now.AddMonths(-1);
            DateTime? end = null;

            var userRepositoryMock = new Mock<UserRepository>();
            var transactionRepositoryMock = new Mock<TransactionRepository>();

            var user = new User { Id = userDTO.Id, Email = "test@example.com" };
            var transactions = new List<Transaction>
            {
                new Transaction { Id = Guid.NewGuid(), UserId = user.Id, Amount = 100, Date = start.AddDays(1) },
                new Transaction { Id = Guid.NewGuid(), UserId = user.Id, Amount = 300, Date = DateTime.Now },
                new Transaction { Id = Guid.NewGuid(), UserId = user.Id, Amount = 200, Date = start.AddDays(-1) },
            };

            userRepositoryMock.Setup(repo => repo.GetUserById(userDTO.Id)).Returns(user);
            transactionRepositoryMock.Setup(repo => repo.GetTransactionsByUserId(user.Id)).Returns(transactions);

            var transactionService = new TransactionService
            {
                UserRepo = userRepositoryMock.Object,
                TransactionRepo = transactionRepositoryMock.Object
            };

            // Act
            var result = transactionService.GetTransactionsInTimePeriod(userDTO, start, end);

            // Assert
            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(transactions.GetRange(0,2), result);
        }

        [TestMethod]
        public void GetTransactionsInTimePeriod_WithValidUserAndInvalidPeriod_ReturnsEmptyList()
        {
            // Arrange
            var userDTO = new UserDTO(new User { Id = Guid.NewGuid() });
            var start = DateTime.Now.AddMonths(1);
            var end = DateTime.Now;

            var userRepositoryMock = new Mock<UserRepository>();
            var transactionRepositoryMock = new Mock<TransactionRepository>();

            var user = new User { Id = userDTO.Id, Email = "test@example.com" };
            var transactions = new List<Transaction>
            {
                new Transaction { Id = Guid.NewGuid(), UserId = user.Id, Amount = 100, Date = start.AddDays(1) },
                new Transaction { Id = Guid.NewGuid(), UserId = user.Id, Amount = 200, Date = start.AddDays(10) }
            };

            userRepositoryMock.Setup(repo => repo.GetUserById(userDTO.Id)).Returns(user);
            transactionRepositoryMock.Setup(repo => repo.GetTransactionsByUserId(user.Id)).Returns(transactions);

            var transactionService = new TransactionService
            {
                UserRepo = userRepositoryMock.Object,
                TransactionRepo = transactionRepositoryMock.Object
            };

            // Act
            var result = transactionService.GetTransactionsInTimePeriod(userDTO, start, end);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void AddTransaction_WithValidParameters_CreatesTransaction()
        {
            // Arrange
            var userDTO = new UserDTO(new User { Id = Guid.NewGuid() });
            var categoryToId = Guid.NewGuid();
            var categoryFromId = Guid.NewGuid();
            var amount = 100M;
            var date = System.DateTime.Now;
            var details = "Transaction details";
            var planned = true;

            var userRepositoryMock = new Mock<UserRepository>();
            var categoryRepositoryMock = new Mock<CategoryRepository>();
            var transactionRepositoryMock = new Mock<TransactionRepository>();

            var user = new User { Id = userDTO.Id, Email = "test@example.com" };
            var categoryTo = new Category { Id = categoryToId, Type = "SomeType" };
            var categoryFrom = new Category { Id = categoryFromId, Type = "SomeType" };

            userRepositoryMock.Setup(repo => repo.GetUserById(userDTO.Id)).Returns(user);
            categoryRepositoryMock.Setup(repo => repo.GetCategoryById(categoryToId)).Returns(categoryTo);
            categoryRepositoryMock.Setup(repo => repo.GetCategoryById(categoryFromId)).Returns(categoryFrom);

            var transactionService = new TransactionService
            {
                UserRepo = userRepositoryMock.Object,
                CategoryRepo = categoryRepositoryMock.Object,
                TransactionRepo = transactionRepositoryMock.Object
            };

            // Act
            transactionService.AddTransaction(userDTO, categoryToId, categoryFromId, amount, date, details, planned);

            // Assert
            transactionRepositoryMock.Verify(repo => repo.CreateTransaction(It.IsAny<Transaction>()), Times.Once);
        }

        [TestMethod]
        public void AddTransaction_WithNullUserDTO_DoesNotCreateTransaction()
        {
            // Arrange
            UserDTO? userDTO = null;

            var userRepositoryMock = new Mock<UserRepository>();
            var categoryRepositoryMock = new Mock<CategoryRepository>();
            var transactionRepositoryMock = new Mock<TransactionRepository>();

            var transactionService = new TransactionService
            {
                UserRepo = userRepositoryMock.Object,
                CategoryRepo = categoryRepositoryMock.Object,
                TransactionRepo = transactionRepositoryMock.Object
            };

            // Act
            transactionService.AddTransaction(userDTO, Guid.NewGuid(), Guid.NewGuid(), 100M, System.DateTime.Now, "Transaction details", true);

            // Assert
            transactionRepositoryMock.Verify(repo => repo.CreateTransaction(It.IsAny<Transaction>()), Times.Never);
        }

        [TestMethod]
        public void AddTransaction_WithNullCategoryTo_DoesNotCreateTransaction()
        {
            // Arrange
            var userDTO = new UserDTO(new User { Id = Guid.NewGuid() });
            var categoryFromId = Guid.NewGuid();
            var amount = 100M;
            var date = System.DateTime.Now;
            var details = "Transaction details";
            var planned = true;

            var userRepositoryMock = new Mock<UserRepository>();
            var categoryRepositoryMock = new Mock<CategoryRepository>();
            var transactionRepositoryMock = new Mock<TransactionRepository>();

            var transactionService = new TransactionService
            {
                UserRepo = userRepositoryMock.Object,
                CategoryRepo = categoryRepositoryMock.Object,
                TransactionRepo = transactionRepositoryMock.Object
            };

            // Act
            transactionService.AddTransaction(userDTO, null, categoryFromId, amount, date, details, planned);

            // Assert
            transactionRepositoryMock.Verify(repo => repo.CreateTransaction(It.IsAny<Transaction>()), Times.Never);
        }

        [TestMethod]
        public void AddTransaction_WithNotNullUserDTO_UserNotFound_DoesNotCreateTransaction()
        {
            // Arrange
            var userDTO = new UserDTO(new User { Id = Guid.NewGuid() });
            var categoryToId = Guid.NewGuid();
            var categoryFromId = Guid.NewGuid();
            var amount = 100M;
            var date = System.DateTime.Now;
            var details = "Transaction details";
            var planned = true;

            var userRepositoryMock = new Mock<UserRepository>();
            var categoryRepositoryMock = new Mock<CategoryRepository>();
            var transactionRepositoryMock = new Mock<TransactionRepository>();
            
            userRepositoryMock.Setup(repo => repo.GetUserById(userDTO.Id)).Returns((User?)null);

            var transactionService = new TransactionService
            {
                UserRepo = userRepositoryMock.Object,
                CategoryRepo = categoryRepositoryMock.Object,
                TransactionRepo = transactionRepositoryMock.Object
            };

            // Act
            transactionService.AddTransaction(userDTO, categoryToId, categoryFromId, amount, date, details, planned);

            // Assert
            transactionRepositoryMock.Verify(repo => repo.CreateTransaction(It.IsAny<Transaction>()), Times.Never);
        }

        [TestMethod]
        public void AddTransaction_WithNotNullCategoryFrom_CategoryNotFound_DoesNotCreateTransaction()
        {
            // Arrange
            var userDTO = new UserDTO(new User { Id = Guid.NewGuid() });
            var categoryToId = Guid.NewGuid();
            var categoryFromId = Guid.NewGuid();
            var amount = 100M;
            var date = System.DateTime.Now;
            var details = "Transaction details";
            var planned = true;

            var userRepositoryMock = new Mock<UserRepository>();
            var categoryRepositoryMock = new Mock<CategoryRepository>();
            var transactionRepositoryMock = new Mock<TransactionRepository>();

            categoryRepositoryMock.Setup(repo => repo.GetCategoryById(categoryFromId)).Returns((Category?)null);

            var transactionService = new TransactionService
            {
                UserRepo = userRepositoryMock.Object,
                CategoryRepo = categoryRepositoryMock.Object,
                TransactionRepo = transactionRepositoryMock.Object
            };

            // Act
            transactionService.AddTransaction(userDTO, categoryToId, categoryFromId, amount, date, details, planned);

            // Assert
            transactionRepositoryMock.Verify(repo => repo.CreateTransaction(It.IsAny<Transaction>()), Times.Never);
        }

        [TestMethod]
        public void UpdateTransaction_WithValidParametersAndMatchingUser_UpdateTransaction()
        {
            // Arrange
            var userDTO = new UserDTO(new User { Id = Guid.NewGuid() });
            var transactionId = Guid.NewGuid();
            var categoryToId = Guid.NewGuid();
            var categoryFromId = Guid.NewGuid();
            decimal amount = 150;
            var date = System.DateTime.Now;
            var details = "Updated details";
            var planned = false;

            var userRepositoryMock = new Mock<UserRepository>();
            var categoryRepositoryMock = new Mock<CategoryRepository>();
            var transactionRepositoryMock = new Mock<TransactionRepository>();

            var user = new User { Id = userDTO.Id, Email = "test@example.com" };
            var categoryTo = new Category { Id = categoryToId, Type = "SomeType" };
            var categoryFrom = new Category { Id = categoryFromId, Type = "SomeType" };
            var existingTransaction = new Transaction
            {
                Id = transactionId,
                UserId = user.Id,
                FromCategory = categoryFromId,
                ToCategory = categoryToId,
                Amount = 100M,
                Date = System.DateTime.Now,
                Details = "Transaction details",
                Planned = true
            };

            userRepositoryMock.Setup(repo => repo.GetUserById(userDTO.Id)).Returns(user);
            categoryRepositoryMock.Setup(repo => repo.GetCategoryById(categoryToId)).Returns(categoryTo);
            categoryRepositoryMock.Setup(repo => repo.GetCategoryById(categoryFromId)).Returns(categoryFrom);
            transactionRepositoryMock.Setup(repo => repo.GetTransactionById(transactionId)).Returns(existingTransaction);

            var transactionService = new TransactionService
            {
                UserRepo = userRepositoryMock.Object,
                CategoryRepo = categoryRepositoryMock.Object,
                TransactionRepo = transactionRepositoryMock.Object
            };

            // Act
            transactionService.UpdateTransaction(userDTO, transactionId, categoryToId, categoryFromId, amount, date, details, planned);

            // Assert
            transactionRepositoryMock.Verify(repo => repo.Update(It.IsAny<Transaction>()), Times.Once);
            Assert.AreEqual(amount, existingTransaction.Amount);
            Assert.AreEqual(date, existingTransaction.Date);
            Assert.AreEqual(details, existingTransaction.Details);
            Assert.AreEqual(planned, existingTransaction.Planned);
        }

        [TestMethod]
        public void UpdateTransaction_WithNullUserDTO_DoesNotUpdateTransaction()
        {
            // Arrange
            UserDTO? userDTO = null;
            var transactionId = Guid.NewGuid();
            var categoryToId = Guid.NewGuid();
            var categoryFromId = Guid.NewGuid();
            decimal amount = 150;
            var date = System.DateTime.Now;
            var details = "Updated details";
            var planned = false;

            var userRepositoryMock = new Mock<UserRepository>();
            var categoryRepositoryMock = new Mock<CategoryRepository>();
            var transactionRepositoryMock = new Mock<TransactionRepository>();

            var transactionService = new TransactionService
            {
                UserRepo = userRepositoryMock.Object,
                CategoryRepo = categoryRepositoryMock.Object,
                TransactionRepo = transactionRepositoryMock.Object
            };

            // Act
            transactionService.UpdateTransaction(userDTO, transactionId, categoryToId, categoryFromId, amount, date, details, planned);

            // Assert
            transactionRepositoryMock.Verify(repo => repo.Update(It.IsAny<Transaction>()), Times.Never);
        }

        [TestMethod]
        public void UpdateTransaction_WithValidUserButNotUserTransaction_DoesNotUpdateTransaction()
        {
            // Arrange
            var userDTO = new UserDTO(new User { Id = Guid.NewGuid() });
            var user = new User { Id = userDTO.Id };
            var transactionId = Guid.NewGuid();
            var categoryToId = Guid.NewGuid();
            var categoryFromId = Guid.NewGuid();
            decimal amount = 150;
            var date = System.DateTime.Now;
            var details = "Updated details";
            var planned = false;

            var userRepositoryMock = new Mock<UserRepository>();
            var categoryRepositoryMock = new Mock<CategoryRepository>();
            var transactionRepositoryMock = new Mock<TransactionRepository>();

            var user2 = new User { Id = Guid.NewGuid(), Email = "otheruser@example.com" }; // Different user than userDTO
            var categoryTo = new Category { Id = categoryToId, Type = "SomeType" };
            var categoryFrom = new Category { Id = categoryFromId, Type = "SomeType" };
            var existingTransaction = new Transaction
            {
                Id = transactionId,
                UserId = user2.Id,
                FromCategory = categoryFromId,
                ToCategory = categoryToId,
                Amount = 100M,
                Date = System.DateTime.Now,
                Details = "Transaction details",
                Planned = true
            };

            userRepositoryMock.Setup(repo => repo.GetUserById(userDTO.Id)).Returns(user);
            categoryRepositoryMock.Setup(repo => repo.GetCategoryById(categoryToId)).Returns(categoryTo);
            categoryRepositoryMock.Setup(repo => repo.GetCategoryById(categoryFromId)).Returns(categoryFrom);
            transactionRepositoryMock.Setup(repo => repo.GetTransactionById(transactionId)).Returns(existingTransaction);

            var transactionService = new TransactionService
            {
                UserRepo = userRepositoryMock.Object,
                CategoryRepo = categoryRepositoryMock.Object,
                TransactionRepo = transactionRepositoryMock.Object
            };

            // Act
            transactionService.UpdateTransaction(userDTO, transactionId, categoryToId, categoryFromId, amount, date, details, planned);

            // Assert
            transactionRepositoryMock.Verify(repo => repo.Update(It.IsAny<Transaction>()), Times.Never);
        }

        [TestMethod]
        public void UpdateTransaction_WithNullTransactionId_DoesNotUpdateTransaction()
        {
            // Arrange
            var userDTO = new UserDTO(new User { Id = Guid.NewGuid() });
            System.Guid? transactionId = null;
            var categoryToId = Guid.NewGuid();
            var categoryFromId = Guid.NewGuid();
            decimal amount = 150;
            var date = System.DateTime.Now;
            var details = "Updated details";
            var planned = false;

            var userRepositoryMock = new Mock<UserRepository>();
            var categoryRepositoryMock = new Mock<CategoryRepository>();
            var transactionRepositoryMock = new Mock<TransactionRepository>();

            var transactionService = new TransactionService
            {
                UserRepo = userRepositoryMock.Object,
                CategoryRepo = categoryRepositoryMock.Object,
                TransactionRepo = transactionRepositoryMock.Object
            };

            // Act
            transactionService.UpdateTransaction(userDTO, transactionId, categoryToId, categoryFromId, amount, date, details, planned);

            // Assert
            transactionRepositoryMock.Verify(repo => repo.Update(It.IsAny<Transaction>()), Times.Never);
        }
        
        [TestMethod]
        public void RemoveTransaction_WithValidUserAndTransactionId_DeletesTransaction()
        {
            // Arrange
            var userDTO = new UserDTO(new User { Id = Guid.NewGuid() });
            var transactionId = Guid.NewGuid();

            var userRepositoryMock = new Mock<UserRepository>();
            var transactionRepositoryMock = new Mock<TransactionRepository>();

            var user = new User { Id = userDTO.Id, Email = "test@example.com" };

            userRepositoryMock.Setup(repo => repo.GetUserById(userDTO.Id)).Returns(user);

            var transactionService = new TransactionService
            {
                UserRepo = userRepositoryMock.Object,
                TransactionRepo = transactionRepositoryMock.Object
            };

            // Act
            transactionService.RemoveTransaction(userDTO, transactionId);

            // Assert
            transactionRepositoryMock.Verify(repo => repo.Delete(transactionId), Times.Once);
        }

        [TestMethod]
        public void RemoveTransaction_WithNullUserDTO_DoesNotDeleteTransaction()
        {
            // Arrange
            UserDTO? userDTO = null;
            var transactionId = Guid.NewGuid();

            var userRepositoryMock = new Mock<UserRepository>();
            var transactionRepositoryMock = new Mock<TransactionRepository>();

            var transactionService = new TransactionService
            {
                UserRepo = userRepositoryMock.Object,
                TransactionRepo = transactionRepositoryMock.Object
            };

            // Act
            transactionService.RemoveTransaction(userDTO, transactionId);

            // Assert
            transactionRepositoryMock.Verify(repo => repo.Delete(It.IsAny<System.Guid>()), Times.Never);
        }

        [TestMethod]
        public void RemoveTransaction_WithValidUserAndNullTransactionId_DoesNotDeleteTransaction()
        {
            // Arrange
            var userDTO = new UserDTO(new User { Id = Guid.NewGuid() });
            System.Guid? transactionId = null;

            var userRepositoryMock = new Mock<UserRepository>();
            var transactionRepositoryMock = new Mock<TransactionRepository>();

            var user = new User { Id = userDTO.Id, Email = "test@example.com" };

            userRepositoryMock.Setup(repo => repo.GetUserById(userDTO.Id)).Returns(user);

            var transactionService = new TransactionService
            {
                UserRepo = userRepositoryMock.Object,
                TransactionRepo = transactionRepositoryMock.Object
            };

            // Act
            transactionService.RemoveTransaction(userDTO, transactionId);

            // Assert
            transactionRepositoryMock.Verify(repo => repo.Delete(It.IsAny<System.Guid>()), Times.Never);
        }

        [TestMethod]
        public void RemoveTransaction_WithNotNullUserDTO_UserNotFound_DoesNotDeleteTransaction()
        {
            // Arrange
            var userDTO = new UserDTO(new User { Id = Guid.NewGuid() });
            var transactionId = Guid.NewGuid();

            var userRepositoryMock = new Mock<UserRepository>();
            var transactionRepositoryMock = new Mock<TransactionRepository>();

            userRepositoryMock.Setup(repo => repo.GetUserById(userDTO.Id)).Returns((User?)null);

            var transactionService = new TransactionService
            {
                UserRepo = userRepositoryMock.Object,
                TransactionRepo = transactionRepositoryMock.Object
            };

            // Act
            transactionService.RemoveTransaction(userDTO, transactionId);

            // Assert
            transactionRepositoryMock.Verify(repo => repo.Delete(It.IsAny<System.Guid>()), Times.Never);
        }

        [TestMethod]
        public void GetTransaction_WithValidUserAndTransaction_ReturnsTransaction()
        {
            // Arrange
            var userDTO = new UserDTO(new User { Id = Guid.NewGuid() });
            var transactionId = Guid.NewGuid();

            var userRepositoryMock = new Mock<UserRepository>();
            var transactionRepositoryMock = new Mock<TransactionRepository>();

            var user = new User { Id = userDTO.Id, Email = "test@example.com" };
            var transaction = new Transaction { Id = transactionId, UserId = user.Id, Amount = 100 };

            userRepositoryMock.Setup(repo => repo.GetUserById(userDTO.Id)).Returns(user);
            transactionRepositoryMock.Setup(repo => repo.GetTransactionById(transactionId)).Returns(transaction);

            var transactionService = new TransactionService
            {
                UserRepo = userRepositoryMock.Object,
                TransactionRepo = transactionRepositoryMock.Object
            };

            // Act
            var result = transactionService.GetTransaction(userDTO, transactionId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(transaction, result);
        }

        [TestMethod]
        public void GetTransaction_WithNullUserDTO_ReturnsNull()
        {
            // Arrange
            UserDTO? userDTO = null;
            var transactionId = Guid.NewGuid();

            var transactionService = new TransactionService();

            // Act
            var result = transactionService.GetTransaction(userDTO, transactionId);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetTransaction_WithValidUserButNullTransactionId_ReturnsNull()
        {
            // Arrange
            var userDTO = new UserDTO(new User { Id = Guid.NewGuid() });
            Guid? transactionId = null;

            var userRepositoryMock = new Mock<UserRepository>();
            var transactionRepositoryMock = new Mock<TransactionRepository>();

            var user = new User { Id = userDTO.Id, Email = "test@example.com" };

            userRepositoryMock.Setup(repo => repo.GetUserById(userDTO.Id)).Returns(user);
            transactionRepositoryMock.Setup(repo => repo.GetTransactionById(It.IsAny<Guid>())).Returns((Transaction?)null);

            var transactionService = new TransactionService
            {
                UserRepo = userRepositoryMock.Object,
                TransactionRepo = transactionRepositoryMock.Object
            };

            // Act
            var result = transactionService.GetTransaction(userDTO, transactionId);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetTransaction_WithValidUserButInvalidTransaction_ReturnsNull()
        {
            // Arrange
            var userDTO = new UserDTO(new User { Id = Guid.NewGuid() });
            var transactionId = Guid.NewGuid();

            var userRepositoryMock = new Mock<UserRepository>();
            var transactionRepositoryMock = new Mock<TransactionRepository>();

            var user = new User { Id = userDTO.Id, Email = "test@example.com" };
            var transaction = new Transaction { Id = Guid.NewGuid(), UserId = user.Id, Amount = 100 };

            userRepositoryMock.Setup(repo => repo.GetUserById(userDTO.Id)).Returns(user);
            transactionRepositoryMock.Setup(repo => repo.GetTransactionById(transactionId)).Returns((Transaction?)null);

            var transactionService = new TransactionService
            {
                UserRepo = userRepositoryMock.Object,
                TransactionRepo = transactionRepositoryMock.Object
            };

            // Act
            var result = transactionService.GetTransaction(userDTO, transactionId);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetTransaction_WithValidUserAndTransactionWithMismatchedUserId_ReturnsNull()
        {
            // Arrange
            var userDTO = new UserDTO(new User { Id = Guid.NewGuid() });
            var transactionId = Guid.NewGuid();

            var userRepositoryMock = new Mock<UserRepository>();
            var transactionRepositoryMock = new Mock<TransactionRepository>();

            var user = new User { Id = userDTO.Id, Email = "test@example.com" };
            var transaction = new Transaction { Id = transactionId, UserId = Guid.NewGuid(), Amount = 100 }; // Different user ID

            userRepositoryMock.Setup(repo => repo.GetUserById(userDTO.Id)).Returns(user);
            transactionRepositoryMock.Setup(repo => repo.GetTransactionById(transactionId)).Returns(transaction);

            var transactionService = new TransactionService
            {
                UserRepo = userRepositoryMock.Object,
                TransactionRepo = transactionRepositoryMock.Object
            };

            // Act
            var result = transactionService.GetTransaction(userDTO, transactionId);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetExpenses_WithValidUserAndTransactions_ReturnsTotalExpenses()
        {
            // Arrange
            var userDTO = new UserDTO(new User { Id = Guid.NewGuid() });

            var userRepositoryMock = new Mock<UserRepository>();
            var categoryRepositoryMock = new Mock<CategoryRepository>();
            var transactionRepositoryMock = new Mock<TransactionRepository>();

            var user = new User { Id = Guid.NewGuid(), Email = "test@example.com" };
            userRepositoryMock.Setup(repo => repo.GetUserById(userDTO.Id)).Returns(user);

            var expenseCategory = new Category { Id = Guid.NewGuid(), Type = "Витрати" };
            categoryRepositoryMock.Setup(repo => repo.GetCategoryById(It.IsAny<System.Guid?>())).Returns(expenseCategory);

            var transactions = new List<Transaction>
            {
                new Transaction { Id = Guid.NewGuid(), UserId = user.Id, Amount = 100, Date = System.DateTime.Now, Planned = false, ToCategory = expenseCategory.Id },
                new Transaction { Id = Guid.NewGuid(), UserId = user.Id, Amount = 200, Date = System.DateTime.Now, Planned = false, ToCategory = expenseCategory.Id }
            };

            transactionRepositoryMock.Setup(repo => repo.GetTransactionsByUserId(user.Id)).Returns(transactions);

            var transactionService = new TransactionService
            {
                UserRepo = userRepositoryMock.Object,
                CategoryRepo = categoryRepositoryMock.Object,
                TransactionRepo = transactionRepositoryMock.Object
            };

            // Act
            var result = transactionService.GetExpenses(userDTO);

            // Assert
            Assert.AreEqual(300, result);
        }

        [TestMethod]
        public void GetExpenses_WithValidUserAndNoTransactions_ReturnsZero()
        {
            // Arrange
            var userDTO = new UserDTO(new User { Id = Guid.NewGuid() });

            var userRepositoryMock = new Mock<UserRepository>();
            var transactionRepositoryMock = new Mock<TransactionRepository>();
            var categoryRepositoryMock = new Mock<CategoryRepository>();

            var user = new User { Id = userDTO.Id, Email = "test@example.com" };

            userRepositoryMock.Setup(repo => repo.GetUserById(userDTO.Id)).Returns(user);
            transactionRepositoryMock.Setup(repo => repo.GetTransactionsByUserId(user.Id)).Returns(new List<Transaction>());
            categoryRepositoryMock.Setup(repo => repo.GetCategoryById(It.IsAny<Guid>())).Returns(new Category { Type = "Витрати" });

            var transactionService = new TransactionService
            {
                UserRepo = userRepositoryMock.Object,
                TransactionRepo = transactionRepositoryMock.Object,
                CategoryRepo = categoryRepositoryMock.Object
            };

            // Act
            var result = transactionService.GetExpenses(userDTO);

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void GetExpenses_WithNullUserDTO_ReturnsZero()
        {
            // Arrange
            UserDTO? userDTO = null;

            var transactionService = new TransactionService();

            // Act
            var result = transactionService.GetExpenses(userDTO);

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void GetBalance_WithValidUserAndTransactions_ReturnsTotalBalance()
        {
            // Arrange
            var userDTO = new UserDTO(new User { Id = Guid.NewGuid() });

            var userRepositoryMock = new Mock<UserRepository>();
            var transactionRepositoryMock = new Mock<TransactionRepository>();
            var categoryRepositoryMock = new Mock<CategoryRepository>();

            var user = new User { Id = userDTO.Id, Email = "test@example.com" };
            var transactions = new List<Transaction>
            {
                new Transaction { Id = Guid.NewGuid(), UserId = user.Id, ToCategory = Guid.NewGuid(), Date = System.DateTime.Now, Amount = 100 },
                new Transaction { Id = Guid.NewGuid(), UserId = user.Id, ToCategory = Guid.NewGuid(), Date = System.DateTime.Now, Amount = 200 },
                new Transaction { Id = Guid.NewGuid(), UserId = user.Id, ToCategory = Guid.NewGuid(), Date = System.DateTime.Now, Amount = 300 }
            };

            userRepositoryMock.Setup(repo => repo.GetUserById(userDTO.Id)).Returns(user);
            transactionRepositoryMock.Setup(repo => repo.GetTransactionsByUserId(user.Id)).Returns(transactions);
            categoryRepositoryMock.Setup(repo => repo.GetCategoryById(It.IsAny<Guid>())).Returns(new Category { Type = "Баланс" });

            var transactionService = new TransactionService
            {
                UserRepo = userRepositoryMock.Object,
                TransactionRepo = transactionRepositoryMock.Object,
                CategoryRepo = categoryRepositoryMock.Object
            };

            // Act
            var result = transactionService.GetBalance(userDTO);

            // Assert
            Assert.AreEqual(600, result);
        }

        [TestMethod]
        public void GetBalance_WithValidUserAndNoTransactions_ReturnsZero()
        {
            // Arrange
            var userDTO = new UserDTO(new User { Id = Guid.NewGuid() });

            var userRepositoryMock = new Mock<UserRepository>();
            var transactionRepositoryMock = new Mock<TransactionRepository>();
            var categoryRepositoryMock = new Mock<CategoryRepository>();

            var user = new User { Id = userDTO.Id, Email = "test@example.com" };

            userRepositoryMock.Setup(repo => repo.GetUserById(userDTO.Id)).Returns(user);
            transactionRepositoryMock.Setup(repo => repo.GetTransactionsByUserId(user.Id)).Returns(new List<Transaction>());
            categoryRepositoryMock.Setup(repo => repo.GetCategoryById(It.IsAny<Guid>())).Returns(new Category { Type = "Баланс" });

            var transactionService = new TransactionService
            {
                UserRepo = userRepositoryMock.Object,
                TransactionRepo = transactionRepositoryMock.Object,
                CategoryRepo = categoryRepositoryMock.Object
            };

            // Act
            var result = transactionService.GetBalance(userDTO);

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void GetBalance_WithNullUserDTO_ReturnsZero()
        {
            // Arrange
            UserDTO? userDTO = null;

            var transactionService = new TransactionService();

            // Act
            var result = transactionService.GetBalance(userDTO);

            // Assert
            Assert.AreEqual(0, result);
        }
    }   
}
