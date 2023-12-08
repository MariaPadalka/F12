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
    public class CategoryServiceTests
    {
        [TestMethod]
        public void GetCategoriesTitleByType_WithValidUserDTO_ReturnsCombinedCategories()
        {
            // Arrange
            var userDTO = new UserDTO(new User { Id = Guid.NewGuid() });
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
                UserRepo = userRepositoryMock.Object,
                CategoryRepo = categoryRepositoryMock.Object
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
                UserRepo = userRepositoryMock.Object,
                CategoryRepo = categoryRepositoryMock.Object
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
}
