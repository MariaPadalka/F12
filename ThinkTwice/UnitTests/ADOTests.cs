using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using DB_Setup;

namespace UnitTests
{
    [TestClass]
    public class ADOTests
    {
        private string? connectionString;

        private readonly List<Guid>? testIds = new List<Guid>();

        [TestInitialize]
        public void TestInitialize()
        {
            connectionString = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build().GetConnectionString("DefaultConnection");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        DeleteTestData(connection, transaction);

                        transaction.Commit();
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        private void DeleteTestData(SqlConnection connection, SqlTransaction transaction)
        {
            if (testIds == null || testIds.Count == 0)
                return; // No test user IDs to delete

            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = $"DELETE FROM Transactions WHERE UserId IN ({string.Join(",", testIds.Select(id => $"'{id}'"))})";
                command.ExecuteNonQuery();
            }

            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = $"DELETE FROM Categories WHERE Id IN ({string.Join(",", testIds.Select(id => $"'{id}'"))})";
                command.ExecuteNonQuery();
            }

            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = $"DELETE FROM Users WHERE Id IN ({string.Join(",", testIds.Select(id => $"'{id}'"))})";
                command.ExecuteNonQuery();
            }
        }

        [TestMethod]
        public void FillUsersTable_WithValidConnectionAndNumberOfUsers_FillsUsersTable()
        {
            // Arrange
            var numberOfUsers = 3;

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                try
                {
                    // Act
                    var userIds = Tables.FillUsersTable(connection, numberOfUsers);

                    // Assert
                    Assert.IsNotNull(userIds);
                    Assert.AreEqual(numberOfUsers, userIds.Length);
                    testIds?.AddRange(userIds);
                }
                finally
                {
                    connection.Close();
                }
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
                var categoryIds = Tables.FillCategoryTable(connection);
                testIds?.AddRange(categoryIds);

                // Assert
                Assert.AreEqual(5, categoryIds.Length);
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
                Assert.ThrowsException<InvalidOperationException>(() => Tables.FillCategoryTable(connection));
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
                Guid[] categoryIds = Tables.FillCategoryTable(connection);
                try
                {
                    // Act
                    Tables.FillTransactionsTable(connection, userIds, categoryIds, 1, 1);
                    
                    // Assert
                    Assert.IsTrue(true);
                    testIds?.AddRange(userIds);
                    testIds?.AddRange(categoryIds);
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
