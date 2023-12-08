using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThinkTwice_Context;
using BLL;
using BLL.DTO;
using Moq;

namespace UnitTests
{
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
                UserRepo = userRepositoryMock.Object
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
                UserRepo = userRepositoryMock.Object
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
                UserRepo = userRepositoryMock.Object
            };

            // Act
            var result = authenticationService.AuthenticateUser(email, password);

            // Assert
            Assert.IsNull(result);
        }
    }
}
