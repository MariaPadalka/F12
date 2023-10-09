using System;
using System.Data.SqlClient;
using Bogus;
using System.Data;

namespace DB_Setup
{
    public class Tables
    {
        public void Fill_Tables()
        {
            // Отримання рядка підключення до бази даних з appsettings.json
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            string connectionString = configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Заповнення таблиці Users
                    FillUsersTable(connection, 3, 30, 50);

                    // Заповнення таблиці Categories
                    FillCategoryTable(connection, 10);

                    // Заповнення таблиці Transactions
                    FillTransactionsTable(connection, 3, 30, 50);

                    // Заповнення таблиці Planning
                    FillPlanningTable(connection, 3, 5);

                    Console.WriteLine("Данi успiшно доданi до бази даних.");
                    Console.WriteLine("Натиснiть будь-яку клавiшу для друку даних з таблиць...");
                    Console.ReadKey();
                    Console.WriteLine("");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Помилка: " + ex.Message);
            }
        }

        // Функція заповнення таблиці Users
        static void FillUsersTable(SqlConnection connection, int numberOfUsers, int minTransactions, int maxTransactions)
        {
            var faker = new Faker();
            using (SqlCommand command = new SqlCommand("INSERT INTO Users (Email, Password, Name, Surname, BirthDate, Currency, AverageIncome) " +
                "VALUES (@Email, @Password, @Name, @Surname, @BirthDate, @Currency, @AverageIncome)", connection))
            {
                for (int i = 0; i < numberOfUsers; i++)
                {
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@Email", faker.Internet.Email());
                    command.Parameters.AddWithValue("@Password", faker.Internet.Password());
                    command.Parameters.AddWithValue("@Name", faker.Name.FirstName());
                    command.Parameters.AddWithValue("@Surname", faker.Name.LastName());
                    command.Parameters.AddWithValue("@BirthDate", faker.Date.Between(new DateTime(1960, 1, 1), new DateTime(2005, 1, 1)));
                    command.Parameters.AddWithValue("@Currency", faker.PickRandom("USD", "EUR", "GBP"));
                    command.Parameters.AddWithValue("@AverageIncome", faker.Random.Decimal(30000, 80000));

                    command.ExecuteNonQuery();

                    // Заповнення таблиці Transactions для поточного користувача
                    FillTransactionsTable(connection, 1, minTransactions, maxTransactions);
                }
            }
        }

        // Функція заповнення таблиці Categories
        static void FillCategoryTable(SqlConnection connection, int numberOfCategories)
        {
            var faker = new Faker();
            using (SqlCommand command = new SqlCommand("INSERT INTO Categories (UserId, Title, IsGeneral) " +
                "VALUES (@UserId, @Title, @IsGeneral)", connection))
            {
                for (int i = 0; i < numberOfCategories; i++)
                {
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@UserId", DBNull.Value); // UserId = null
                    command.Parameters.AddWithValue("@Title", faker.Commerce.Department());
                    command.Parameters.AddWithValue("@IsGeneral", faker.Random.Bool() ? 1 : 0);

                    command.ExecuteNonQuery();
                }
            }
        }

        // Функція заповнення таблиці Transactions
        static void FillTransactionsTable(SqlConnection connection, int userId, int minTransactions, int maxTransactions)
        {
            var faker = new Faker();
            using (SqlCommand command = new SqlCommand("INSERT INTO Transactions (UserId, CategoryId, Amount, Date, Title, Details, Planned) " +
                "VALUES (@UserId, @CategoryId, @Amount, @Date, @Title, @Details, @Planned)", connection))
            {
                for (int i = 0; i < faker.Random.Number(minTransactions, maxTransactions); i++)
                {
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@CategoryId", faker.Random.Number(1, 10)); // Замініть 10 на кількість категорій
                    command.Parameters.AddWithValue("@Amount", faker.Random.Decimal(1, 1000));
                    command.Parameters.AddWithValue("@Date", faker.Date.Past());
                    command.Parameters.AddWithValue("@Title", faker.Commerce.ProductName());
                    command.Parameters.AddWithValue("@Details", faker.Lorem.Sentence());
                    command.Parameters.AddWithValue("@Planned", faker.Random.Bool() ? 1 : 0);

                    command.ExecuteNonQuery();
                }
            }
        }

        // Функція заповнення таблиці Planning
        static void FillPlanningTable(SqlConnection connection, int numberOfUsers, int numberOfCategories)
        {
            var faker = new Faker();
            using (SqlCommand command = new SqlCommand("INSERT INTO Planning (UserId, CategoryId, PercentageAmount) " +
                "VALUES (@UserId, @CategoryId, @PercentageAmount)", connection))
            {
                for (int i = 1; i <= numberOfUsers; i++)
                {
                    for (int j = 1; j <= numberOfCategories; j++)
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@UserId", i);
                        command.Parameters.AddWithValue("@CategoryId", j);
                        command.Parameters.AddWithValue("@PercentageAmount", faker.Random.Decimal(0, 100));

                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
