using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThinkTwice_Context;
using BLL;
using BLL.DTO;
using Moq;
using System.Collections.Generic;
using System;
using System.Linq;

namespace UnitTests
{
    [TestClass]
    public class SettingsServiceTests
    {
        [TestMethod]
        public void UpdateUser_WithValidUserDTO_ReturnsUpdatedUserDTO()
        {
            // Arrange
            var updatedUserDTO = new UserDTO
            ( new User {
                Id = Guid.NewGuid(),
                Email = "updated@example.com",
                Surname = "UpdatedSurname",
                Name = "UpdatedName",
                BirthDate = new DateTime(1990, 1, 1),
                Currency = "USD",
                Categories = new List<Category>
                {
                    new Category { Id = Guid.NewGuid(), Title = "Category1", Type = "Expense" },
                    new Category { Id = Guid.NewGuid(), Title = "Category2", Type = "Income" }
                }
            });

            var userRepositoryMock = new Mock<UserRepository>();
            userRepositoryMock.Setup(repo => repo.GetUserById(updatedUserDTO.Id)).Returns(new User { Id = updatedUserDTO.Id });

            var settingsService = new SettingsService
            {
                UserRepo = userRepositoryMock.Object
            };

            // Act
            var result = settingsService.UpdateUser(updatedUserDTO);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(updatedUserDTO, result);
        }

        [TestMethod]
        public void UpdateUser_WithNullUserDTO_ReturnsNull()
        {
            // Arrange
            UserDTO? updatedUserDTO = null;

            var settingsService = new SettingsService();

            // Act
            var result = settingsService.UpdateUser(updatedUserDTO);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void UpdateUser_WithNonExistingUser_ReturnsNull()
        {
            // Arrange
            var updatedUserDTO = new UserDTO(new User { Id = Guid.NewGuid() });

            var userRepositoryMock = new Mock<UserRepository>();
            userRepositoryMock.Setup(repo => repo.GetUserById(updatedUserDTO.Id)).Returns((User?)null);

            var settingsService = new SettingsService
            {
                UserRepo = userRepositoryMock.Object
            };

            // Act
            var result = settingsService.UpdateUser(updatedUserDTO);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetUserCategories_WithValidUser_ReturnsUserAndGeneralCategories()
        {
            // Arrange
            var userDTO = new UserDTO(new User { Id = Guid.NewGuid() });

            var userRepositoryMock = new Mock<UserRepository>();
            var categoryRepositoryMock = new Mock<CategoryRepository>();
            var settingsService = new SettingsService
            {
                UserRepo = userRepositoryMock.Object,
                CategoryRepo = categoryRepositoryMock.Object
            };

            var user = new User { Id = userDTO.Id, Email = "test@example.com" };
            var userCategories = new List<Category>
            {
                new Category { Id = Guid.NewGuid(), Title = "UserCategory1", UserId = user.Id },
                new Category { Id = Guid.NewGuid(), Title = "UserCategory2", UserId = user.Id }
            };
            var generalCategories = new List<Category>
            {
                new Category { Id = Guid.NewGuid(), Title = "GeneralCategory1", UserId = null },
                new Category { Id = Guid.NewGuid(), Title = "GeneralCategory2", UserId = null }
            };

            userRepositoryMock.Setup(repo => repo.GetUserById(userDTO.Id)).Returns(user);
            categoryRepositoryMock.Setup(repo => repo.GetCategoriesByUserId(user.Id)).Returns(userCategories);
            categoryRepositoryMock.Setup(repo => repo.GetGeneralCategories()).Returns(generalCategories);

            // Act
            var result = settingsService.GetUserCategories(userDTO);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Count());
        }

        [TestMethod]
        public void GetUserCategories_WithNullUserDTO_ReturnsGeneralCategories()
        {
            // Arrange
            UserDTO? userDTO = null;

            var categoryRepositoryMock = new Mock<CategoryRepository>();
            var settingsService = new SettingsService { CategoryRepo = categoryRepositoryMock.Object };

            var generalCategories = new List<Category>
            {
                new Category { Id = Guid.NewGuid(), Title = "GeneralCategory1", UserId = null },
                new Category { Id = Guid.NewGuid(), Title = "GeneralCategory2", UserId = null }
            };

            categoryRepositoryMock.Setup(repo => repo.GetGeneralCategories()).Returns(generalCategories);

            // Act
            var result = settingsService.GetUserCategories(userDTO);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
        }

        [TestMethod]
        public void GetUserCategories_WithInvalidUser_ReturnsNull()
        {
            // Arrange
            var userDTO = new UserDTO(new User { Id = Guid.NewGuid() });

            var userRepositoryMock = new Mock<UserRepository>();
            var categoryRepositoryMock = new Mock<CategoryRepository>();
            var settingsService = new SettingsService
            {
                UserRepo = userRepositoryMock.Object,
                CategoryRepo = categoryRepositoryMock.Object
            };

            userRepositoryMock.Setup(repo => repo.GetUserById(userDTO.Id)).Returns((User?)null);

            // Act
            var result = settingsService.GetUserCategories(userDTO);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void RemoveCategory_WithValidUserAndCategoryToRemove_RemovesCategory()
        {
            // Arrange
            var userDTO = new UserDTO ( new User { Id = Guid.NewGuid() });
            var categoryId = Guid.NewGuid();

            var userRepositoryMock = new Mock<UserRepository>();
            var categoryRepositoryMock = new Mock<CategoryRepository>();

            var user = new User { Id = userDTO.Id, Email = "test@example.com" };
            var category = new Category { Id = categoryId, UserId = userDTO.Id, Title = "Category1", Type = "Expense" };

            userRepositoryMock.Setup(repo => repo.GetUserById(userDTO.Id)).Returns(user);
            categoryRepositoryMock.Setup(repo => repo.GetCategoryById(categoryId)).Returns(category);

            var settingsService = new SettingsService
            {
                UserRepo = userRepositoryMock.Object,
                CategoryRepo = categoryRepositoryMock.Object
            };


            // Act
            settingsService.RemoveCategory(userDTO, categoryId);

            // Assert (no exception should be thrown)
            categoryRepositoryMock.Verify(repo => repo.Delete(It.IsAny<Guid>()), Times.Once);
        }

        [TestMethod]
        public void RemoveCategory_WithValidUserAndNonExistingCategory_DoesNotRemomveCategory()
        {
            // Arrange
            var userDTO = new UserDTO(new User { Id = Guid.NewGuid() });
            var categoryId = Guid.NewGuid();

            var userRepositoryMock = new Mock<UserRepository>();
            var categoryRepositoryMock = new Mock<CategoryRepository>();

            var user = new User { Id = userDTO.Id, Email = "test@example.com" };

            userRepositoryMock.Setup(repo => repo.GetUserById(userDTO.Id)).Returns(user);
            categoryRepositoryMock.Setup(repo => repo.GetCategoryById(categoryId)).Returns((Category?)null);

            var settingsService = new SettingsService
            {
                UserRepo = userRepositoryMock.Object,
                CategoryRepo = categoryRepositoryMock.Object
            };

            // Act
            settingsService.RemoveCategory(userDTO, categoryId);

            // Assert
            categoryRepositoryMock.Verify(repo => repo.Delete(It.IsAny<Guid>()), Times.Never);
        }

        [TestMethod]
        public void RemoveCategory_WithNullUserDTO_DoesNotRemomveCategory()
        {
            // Arrange
            UserDTO? userDTO = null;
            var categoryId = Guid.NewGuid();

            var userRepositoryMock = new Mock<UserRepository>();
            var categoryRepositoryMock = new Mock<CategoryRepository>();

            var settingsService = new SettingsService
            {
                UserRepo = userRepositoryMock.Object,
                CategoryRepo = categoryRepositoryMock.Object
            };

            // Act
            settingsService.RemoveCategory(userDTO, categoryId);

            // Assert
            categoryRepositoryMock.Verify(repo => repo.Delete(It.IsAny<Guid>()), Times.Never);
        }

        [TestMethod]
        public void RemoveCategory_WithValidUserAndCategoryBelongsToAnotherUse_DoesNotRemomveCategory()
        {
            // Arrange
            var userDTO = new UserDTO(new User { Id = Guid.NewGuid() });
            var categoryId = Guid.NewGuid();

            var userRepositoryMock = new Mock<UserRepository>();
            var categoryRepositoryMock = new Mock<CategoryRepository>();

            var user = new User { Id = userDTO.Id, Email = "test@example.com" };
            var category = new Category { Id = categoryId, UserId = Guid.NewGuid(), Title = "Category1", Type = "Витрати" };

            userRepositoryMock.Setup(repo => repo.GetUserById(userDTO.Id)).Returns(user);
            categoryRepositoryMock.Setup(repo => repo.GetCategoryById(categoryId)).Returns(category);

            var settingsService = new SettingsService
            {
                UserRepo = userRepositoryMock.Object,
                CategoryRepo = categoryRepositoryMock.Object
            };

            // Act
            settingsService.RemoveCategory(userDTO, categoryId);

            // Assert
            categoryRepositoryMock.Verify(repo => repo.Delete(It.IsAny<Guid>()), Times.Never);
        }

        [TestMethod]
        public void UniqueEmail_WithNonExistingEmail_ReturnsTrue()
        {
            // Arrange
            var email = "newuser@example.com";

            var userRepositoryMock = new Mock<UserRepository>();
            userRepositoryMock.Setup(repo => repo.GetUserByEmail(email)).Returns((User?)null);

            var settingsService = new SettingsService
            {
                UserRepo = userRepositoryMock.Object
            };

            // Act
            var result = settingsService.UniqueEmail(email);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void UniqueEmail_WithExistingEmail_ReturnsFalse()
        {
            // Arrange
            var email = "existinguser@example.com";

            var userRepositoryMock = new Mock<UserRepository>();
            userRepositoryMock.Setup(repo => repo.GetUserByEmail(email)).Returns(new User { Id = Guid.NewGuid(), Email = email });

            var settingsService = new SettingsService
            {
                UserRepo = userRepositoryMock.Object
            };

            // Act
            var result = settingsService.UniqueEmail(email);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void UniqueEmail_WithNullEmail_ReturnsFalse()
        {
            // Arrange
            string? email = null;

            var settingsService = new SettingsService();

            // Act
            var result = settingsService.UniqueEmail(email);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
