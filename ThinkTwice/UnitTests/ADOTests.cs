using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using DB_Setup;
using System;
using Microsoft.Extensions.Configuration;

namespace UnitTests
{
    [TestClass]
    public class ADOTests
    {
        private string? connectionString = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build().GetConnectionString("DefaultConnection");

        [TestMethod]
        public void FillUsersTable_WithValidConnectionAndNumberOfUsers_FillsUsersTable()
        {
            // Arrange
            var numberOfUsers = 3;

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Act
                var userIds = Tables.FillUsersTable(connection, numberOfUsers);

                // Assert
                Assert.IsNotNull(userIds);
                Assert.AreEqual(numberOfUsers, userIds.Length);
                connection.Close();
            }
        }

        [TestMethod]
        public void FillUsersTable_WithInvalidConnection_ThrowsException()
        {
            // Arrange
            string invalidConnectionString = "Data Source=your_server;Initial Catalog=your_database;User Id=your_username;Password=your_password;";

            using (var connection = new SqlConnection(invalidConnectionString))
            {
                Assert.AreEqual(System.Data.ConnectionState.Closed, connection.State);

                // Act & Assert
                Assert.ThrowsException<InvalidOperationException>(() => Tables.FillUsersTable(connection, 3));
            }
        }

        [TestMethod]
        public void FillCategoryTable_WithValidConnection_FillsCategories()
        {
            // Arrange
            string? validConnectionString = connectionString;

            using (var connection = new SqlConnection(validConnectionString))
            {
                connection.Open();

                // Act
                var categoryIds = Tables.FillCategoryTable(connection, 7);

                // Assert
                Assert.AreEqual(7, categoryIds.Length);
                CollectionAssert.AllItemsAreUnique(categoryIds);
                connection.Close();
            }
        }

        [TestMethod]
        public void FillCategoryTable_WithInvalidConnection_ThrowsException()
        {
            // Arrange
            string invalidConnectionString = "Data Source=your_server;Initial Catalog=your_database;User Id=your_username;Password=your_password;";

            using (var connection = new SqlConnection(invalidConnectionString))
            {
                // Ensure the connection is closed initially
                Assert.AreEqual(System.Data.ConnectionState.Closed, connection.State);

                // Act & Assert
                Assert.ThrowsException<InvalidOperationException>(() => Tables.FillCategoryTable(connection, 3));
            }
        }

        [TestMethod]
        public void FillTransactionsTable_WithValidConnection_FillsTransactions()
        {
            // Arrange
            string? validConnectionString = connectionString;
            using (var connection = new SqlConnection(validConnectionString))
            {
                connection.Open();

                Guid[] userIds = Tables.FillUsersTable(connection, 1);
                Guid[] categoryIds = Tables.FillCategoryTable(connection, 1);
                try
                {
                    // Act
                    Tables.FillTransactionsTable(connection, userIds, categoryIds, 1, 1);

                    // Assert
                    Assert.IsTrue(true);
                }
                catch (Exception ex)
                {
                    Assert.Fail($"Unexpected exception: {ex.Message}");
                }
                connection.Close();
            }
        }

        [TestMethod]
        public void FillTransactionsTable_WithInvalidConnection_ThrowsException()
        {
            // Arrange
            string invalidConnectionString = "Data Source=your_server;Initial Catalog=your_database;User Id=your_username;Password=your_password;";

            Guid[] userIds = { Guid.NewGuid() };
            Guid[] categoryIds = { Guid.NewGuid() };
            using (var connection = new SqlConnection(invalidConnectionString))
            {
                // Ensure the connection is closed initially
                Assert.AreEqual(System.Data.ConnectionState.Closed, connection.State);

                // Act & Assert
                Assert.ThrowsException<InvalidOperationException>(() => Tables.FillTransactionsTable(connection, userIds, categoryIds, 1, 1));
            }
        }
    }
}
