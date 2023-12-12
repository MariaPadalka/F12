using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThinkTwice_Context;
using BLL;
using BLL.DTO;
using Moq;

namespace UnitTests
{
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
                UserRepo = userRepositoryMock.Object
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
                UserRepo = userRepositoryMock.Object
            };

            // Act
            var result = registrationService.Register(email, password, firstName, lastName, birthDate, currency);

            // Assert
            Assert.IsNull(result);
        }
    }
}
