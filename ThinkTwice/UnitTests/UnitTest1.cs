using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bogus;
using Bogus.DataSets;
using ThinkTwice_Context;
using BLL;
using BLL.DTO;
using Moq;
using System.Collections.Generic;
using System;

namespace UnitTests
{
    [TestClass]
    public class CategoryServiceTests
    {
        [TestMethod]
        public void GetCategoriesTitleByType_WithValidUserDTO_ReturnsCombinedCategories()
        {
            // Arrange
            var userDTO = new UserDTO(new User { Id = Guid.NewGuid()});
            var type = "Витрати";

            var userRepositoryMock = new Mock<UserRepository>();
            var categoryRepositoryMock = new Mock<CategoryRepository>();

            userRepositoryMock.Setup(repo => repo.GetUserById(It.IsAny<System.Guid>())).Returns(new User { Id = Guid.NewGuid() });

            var userCategories = new List<Category>
            {
                new Category { Id = Guid.NewGuid(), Title = "UserCategory1", Type = "Витрати" },
                new Category { Id = Guid.NewGuid(), Title = "UserCategory2", Type = "Дохід" }
            };

            var generalCategories = new List<Category>
            {
                new Category { Id = Guid.NewGuid(), Title = "GeneralCategory1", Type = "Витрати" },
                new Category { Id = Guid.NewGuid(), Title = "GeneralCategory2", Type = "Витрати" },
                new Category { Id = Guid.NewGuid(), Title = "GeneralCategory3", Type = "Баланс" },
                new Category { Id = Guid.NewGuid(), Title = "GeneralCategory4", Type = "Дохід" }
            };

            categoryRepositoryMock.Setup(repo => repo.GetCategoriesByType(It.IsAny<System.Guid>(), It.IsAny<string>())).Returns(userCategories);
            categoryRepositoryMock.Setup(repo => repo.GetGeneralCategories()).Returns(generalCategories);

            var categoryService = new CategoryService
            {
                userRepository = userRepositoryMock.Object,
                categoryRepository = categoryRepositoryMock.Object
            };

            // Act
            var result = categoryService.GetCategoriesTitleByType(userDTO, type);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);
            CollectionAssert.Contains(result, "UserCategory1");
            CollectionAssert.Contains(result, "GeneralCategory1");
            CollectionAssert.Contains(result, "GeneralCategory2");
        }
    
        [TestMethod]
        public void GetCategoriesTitleByType_WithNullUserDTO_ReturnsGeneralCategories()
        {
            // Arrange
            var type = "Дохід";

            var categoryRepositoryMock = new Mock<CategoryRepository>();
            var userRepositoryMock = new Mock<UserRepository>();

            var generalCategories = new List<Category>
            {
                new Category { Id = Guid.NewGuid(), Title = "GeneralCategory1", Type = "Дохід" },
                new Category { Id = Guid.NewGuid(), Title = "GeneralCategory2", Type = "Дохід" },
                new Category { Id = Guid.NewGuid(), Title = "GeneralCategory3", Type = "Витрати" },
                new Category { Id = Guid.NewGuid(), Title = "GeneralCategory4", Type = "Баланс" }
            };

            categoryRepositoryMock.Setup(repo => repo.GetGeneralCategories()).Returns(generalCategories);

            var categoryService = new CategoryService
            {
                userRepository = userRepositoryMock.Object,
                categoryRepository = categoryRepositoryMock.Object
            };

            // Act
            var result = categoryService.GetCategoriesTitleByType(null, type);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count); // Only general categories for the specified type
            CollectionAssert.Contains(result, "GeneralCategory1");
            CollectionAssert.Contains(result, "GeneralCategory2");
        }

    }

    [TestClass]
    public class RegistrationServiceTests
    {
        [TestMethod]
        public void Register_WithUniqueEmail_ReturnsUserDTO()
        {
            // Arrange
            var email = "test@example.com";
            var password = "password";
            var firstName = "John";
            var lastName = "Doe";
            var birthDate = new System.DateTime(1990, 1, 1);
            var currency = "USD";

            var userRepositoryMock = new Mock<UserRepository>();
            userRepositoryMock.Setup(repo => repo.GetUserByEmail(It.IsAny<string>())).Returns((User?)null);

            var registrationService = new RegistrationService
            {
                userService = userRepositoryMock.Object
            };

            // Act
            var result = registrationService.Register(email, password, firstName, lastName, birthDate, currency);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(UserDTO));
            Assert.AreEqual(email, result!.Email);
            Assert.AreEqual(firstName, result.Name);
            Assert.AreEqual(lastName, result.Surname);
            Assert.AreEqual(birthDate, result.BirthDate);
            Assert.AreEqual(currency, result.Currency);
        }

        [TestMethod]
        public void Register_WithDuplicateEmail_ReturnsNull()
        {
            // Arrange
            var email = "test@example.com";
            var password = "password";
            var firstName = "John";
            var lastName = "Doe";
            var birthDate = new System.DateTime(1990, 1, 1);
            var currency = "USD";

            var existingUser = new User
            {
                Email = email,
                Password = "hashedPassword",
                Name = "Existing",
                Surname = "User",
                BirthDate = birthDate,
                Currency = currency
            };

            var userRepositoryMock = new Mock<UserRepository>();
            userRepositoryMock.Setup(repo => repo.GetUserByEmail(email)).Returns(existingUser);

            var registrationService = new RegistrationService
            {
                userService = userRepositoryMock.Object
            };

            // Act
            var result = registrationService.Register(email, password, firstName, lastName, birthDate, currency);

            // Assert
            Assert.IsNull(result);
        }
    }

    [TestClass]
    public class AuthenticationServiceTests
    {
        [TestMethod]
        public void AuthenticateUser_WithValidCredentials_ReturnsUserDTO()
        {
            // Arrange
            var email = "test@example.com";
            var password = "password";

            var existingUser = new User
            {
                Email = email,
                Password = PasswordHasher.HashPassword(password),
                Name = "John",
                Surname = "Doe",
                BirthDate = new System.DateTime(1990, 1, 1),
                Currency = "USD"
            };

            var userRepositoryMock = new Mock<UserRepository>();
            userRepositoryMock.Setup(repo => repo.GetUserByEmail(email)).Returns(existingUser);

            var authenticationService = new AuthenticationService
            {
                userService = userRepositoryMock.Object
            };

            // Act
            var result = authenticationService.AuthenticateUser(email, password);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(UserDTO));
            Assert.AreEqual(email, result!.Email);
        }

        [TestMethod]
        public void AuthenticateUser_WithInvalidPassword_ReturnsNull()
        {
            // Arrange
            var email = "test@example.com";
            var password = "password";

            var existingUser = new User
            {
                Email = email,
                Password = PasswordHasher.HashPassword("differentpassword"),
                Name = "John",
                Surname = "Doe",
                BirthDate = new System.DateTime(1990, 1, 1),
                Currency = "USD"
            };

            var userRepositoryMock = new Mock<UserRepository>();
            userRepositoryMock.Setup(repo => repo.GetUserByEmail(email)).Returns(existingUser);

            var authenticationService = new AuthenticationService
            {
                userService = userRepositoryMock.Object
            };

            // Act
            var result = authenticationService.AuthenticateUser(email, password);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void AuthenticateUser_WithNonexistentUser_ReturnsNull()
        {
            // Arrange
            var email = "nonexistent@example.com";
            var password = "password";

            var userRepositoryMock = new Mock<UserRepository>();
            userRepositoryMock.Setup(repo => repo.GetUserByEmail(email)).Returns((User?)null);

            var authenticationService = new AuthenticationService
            {
                userService = userRepositoryMock.Object
            };

            // Act
            var result = authenticationService.AuthenticateUser(email, password);

            // Assert
            Assert.IsNull(result);
        }
    }

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
                userRepository = userRepositoryMock.Object,
                transactionRepository = transactionRepositoryMock.Object
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
                userRepository = userRepositoryMock.Object,
                transactionRepository = transactionRepositoryMock.Object
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
                userRepository = userRepositoryMock.Object,
                transactionRepository = transactionRepositoryMock.Object
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
                userRepository = userRepositoryMock.Object,
                transactionRepository = transactionRepositoryMock.Object
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
                userRepository = userRepositoryMock.Object,
                transactionRepository = transactionRepositoryMock.Object
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
                userRepository = userRepositoryMock.Object,
                transactionRepository = transactionRepositoryMock.Object
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
                userRepository = userRepositoryMock.Object,
                transactionRepository = transactionRepositoryMock.Object
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
                userRepository = userRepositoryMock.Object,
                categoryRepository = categoryRepositoryMock.Object,
                transactionRepository = transactionRepositoryMock.Object
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
                userRepository = userRepositoryMock.Object,
                categoryRepository = categoryRepositoryMock.Object,
                transactionRepository = transactionRepositoryMock.Object
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
                userRepository = userRepositoryMock.Object,
                categoryRepository = categoryRepositoryMock.Object,
                transactionRepository = transactionRepositoryMock.Object
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
                userRepository = userRepositoryMock.Object,
                categoryRepository = categoryRepositoryMock.Object,
                transactionRepository = transactionRepositoryMock.Object
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
                userRepository = userRepositoryMock.Object,
                categoryRepository = categoryRepositoryMock.Object,
                transactionRepository = transactionRepositoryMock.Object
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
                userRepository = userRepositoryMock.Object,
                categoryRepository = categoryRepositoryMock.Object,
                transactionRepository = transactionRepositoryMock.Object
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
                userRepository = userRepositoryMock.Object,
                categoryRepository = categoryRepositoryMock.Object,
                transactionRepository = transactionRepositoryMock.Object
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
                userRepository = userRepositoryMock.Object,
                categoryRepository = categoryRepositoryMock.Object,
                transactionRepository = transactionRepositoryMock.Object
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
                userRepository = userRepositoryMock.Object,
                categoryRepository = categoryRepositoryMock.Object,
                transactionRepository = transactionRepositoryMock.Object
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
                userRepository = userRepositoryMock.Object,
                transactionRepository = transactionRepositoryMock.Object
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
                userRepository = userRepositoryMock.Object,
                transactionRepository = transactionRepositoryMock.Object
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
                userRepository = userRepositoryMock.Object,
                transactionRepository = transactionRepositoryMock.Object
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
                userRepository = userRepositoryMock.Object,
                transactionRepository = transactionRepositoryMock.Object
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
                userRepository = userRepositoryMock.Object,
                transactionRepository = transactionRepositoryMock.Object
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
            transactionRepositoryMock.Setup(repo => repo.GetTransactionById(It.IsAny<Guid>())).Returns<Transaction>(null);

            var transactionService = new TransactionService
            {
                userRepository = userRepositoryMock.Object,
                transactionRepository = transactionRepositoryMock.Object
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
                userRepository = userRepositoryMock.Object,
                transactionRepository = transactionRepositoryMock.Object
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
                userRepository = userRepositoryMock.Object,
                transactionRepository = transactionRepositoryMock.Object
            };

            // Act
            var result = transactionService.GetTransaction(userDTO, transactionId);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetExpenses_WithValidUser_ReturnsTotalExpenses()
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
                userRepository = userRepositoryMock.Object,
                categoryRepository = categoryRepositoryMock.Object,
                transactionRepository = transactionRepositoryMock.Object
            };

            // Act
            var result = transactionService.GetExpenses(userDTO);

            // Assert
            Assert.AreEqual(300, result);
        }
    }   
}
