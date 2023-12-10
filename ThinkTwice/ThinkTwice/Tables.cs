namespace DB_Setup
{
    using System.Data.SqlClient;
    using Bogus;
    using Microsoft.Extensions.Configuration;
    using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

    public enum TransType
    {
        Дохід,
        Витрати,
        Баланс,
    }

    public class Tables
    {
        private readonly int numberOfUsers = 3;
        private int numberOfCategories = 7;

        public static void DisplayDataFromTable(SqlConnection connection, string tableName)
        {
            using (SqlCommand command = new SqlCommand($"SELECT * FROM {tableName}", connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.WriteLine($"{reader.GetName(i)}: {reader[i]}");
                        }

                        Console.WriteLine("---------------");
                    }
                }
            }
        }

        public static Guid[] FillUsersTable(SqlConnection connection, int numberOfUsers)
        {
            var faker = new Faker();
            Guid[] userIds = new Guid[numberOfUsers];

            using (SqlCommand command = new SqlCommand(
                "INSERT INTO Users (Email, Password, Name, Surname, BirthDate, Currency) " +
                "OUTPUT INSERTED.Id VALUES (@Email, @Password, @Name, @Surname, @BirthDate, @Currency)", connection))
            {
                for (int i = 0; i < numberOfUsers; i++)
                {
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@Email", faker.Internet.Email());
                    string password = faker.Internet.Password();
                    command.Parameters.AddWithValue("@Password", BCrypt.Net.BCrypt.HashPassword(password));
                    command.Parameters.AddWithValue("@Name", faker.Name.FirstName());
                    command.Parameters.AddWithValue("@Surname", faker.Name.LastName());
                    command.Parameters.AddWithValue("@BirthDate", faker.Date.Between(new DateTime(1960, 1, 1), new DateTime(2005, 1, 1)));
                    command.Parameters.AddWithValue("@Currency", faker.PickRandom("USD", "EUR", "GBP"));

                    userIds[i] = (Guid)command.ExecuteScalar();
                }
            }

            return userIds;
        }

        public static Guid[] FillCategoryTable(SqlConnection connection, int numberOfCategories)
        {
            var faker = new Faker();
            //Guid[] categoryIds = new Guid[7];
            Guid[] categoryIds = new Guid[4];

            using (SqlCommand command = new SqlCommand(
                "INSERT INTO Categories (UserId, Title, IsGeneral, PercentageAmount, Type) " +
                "OUTPUT INSERTED.Id VALUES (@UserId, @Title, @IsGeneral, @PercentageAmount, @Type)", connection))
            {
                //command.Parameters.Clear();
                //command.Parameters.AddWithValue("@UserId", DBNull.Value);
                //command.Parameters.AddWithValue("@Title", "Їжа");
                //command.Parameters.AddWithValue("@IsGeneral", 1);
                //command.Parameters.AddWithValue("@PercentageAmount", 0);
                //command.Parameters.AddWithValue("@Type", TransType.Витрати.ToString());

                //categoryIds[0] = (Guid)command.ExecuteScalar();
                //command.Parameters.Clear();
                //command.Parameters.AddWithValue("@UserId", DBNull.Value);
                //command.Parameters.AddWithValue("@Title", "Одяг");
                //command.Parameters.AddWithValue("@IsGeneral", 1);
                //command.Parameters.AddWithValue("@PercentageAmount", 0);
                //command.Parameters.AddWithValue("@Type", TransType.Витрати.ToString());

                //categoryIds[1] = (Guid)command.ExecuteScalar();

                //command.Parameters.Clear();
                //command.Parameters.AddWithValue("@UserId", DBNull.Value);
                //command.Parameters.AddWithValue("@Title", "Комунальні послуги");
                //command.Parameters.AddWithValue("@IsGeneral", 1);
                //command.Parameters.AddWithValue("@PercentageAmount", 0);
                //command.Parameters.AddWithValue("@Type", TransType.Витрати.ToString());

                //categoryIds[2] = (Guid)command.ExecuteScalar();

                command.Parameters.Clear();
                command.Parameters.AddWithValue("@UserId", DBNull.Value);
                command.Parameters.AddWithValue("@Title", "Зарплата");
                command.Parameters.AddWithValue("@IsGeneral", 1);
                command.Parameters.AddWithValue("@PercentageAmount", 0);
                command.Parameters.AddWithValue("@Type", TransType.Дохід.ToString());

                categoryIds[0] = (Guid)command.ExecuteScalar();

                command.Parameters.Clear();
                command.Parameters.AddWithValue("@UserId", DBNull.Value);
                command.Parameters.AddWithValue("@Title", "Стипендія");
                command.Parameters.AddWithValue("@IsGeneral", 1);
                command.Parameters.AddWithValue("@PercentageAmount", 0);
                command.Parameters.AddWithValue("@Type", TransType.Дохід.ToString());

                categoryIds[1] = (Guid)command.ExecuteScalar();
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@UserId", DBNull.Value);
                command.Parameters.AddWithValue("@Title", "Готівка");
                command.Parameters.AddWithValue("@IsGeneral", 1);
                command.Parameters.AddWithValue("@PercentageAmount", 0);
                command.Parameters.AddWithValue("@Type", TransType.Баланс.ToString());

                categoryIds[2] = (Guid)command.ExecuteScalar();
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@UserId", DBNull.Value);
                command.Parameters.AddWithValue("@Title", "Картка");
                command.Parameters.AddWithValue("@IsGeneral", 1);
                command.Parameters.AddWithValue("@PercentageAmount", 0);
                command.Parameters.AddWithValue("@Type", TransType.Баланс.ToString());

                categoryIds[3] = (Guid)command.ExecuteScalar();
            }

            return categoryIds;
        }

        public static void FillTransactionsTable(SqlConnection connection, Guid[] userIds, Guid[] categoryIds, int minTransactions, int maxTransactions)
        {
            var faker = new Faker();
            using (SqlCommand command = new SqlCommand(
                "INSERT INTO Transactions (UserId, From_category, To_category, Amount, Date, Details, Planned) " +
                "VALUES (@UserId, @From_category, @To_category, @Amount, @Date, @Details, @Planned)", connection))
            {
                for (int i = 0; i < userIds.Length; i++)
                {
                    for (int j = 0; j < faker.Random.Number(minTransactions, maxTransactions); j++)
                    {
                        command.Parameters.Clear();
                        command.Parameters.AddWithValue("@UserId", userIds[i]);
                        command.Parameters.AddWithValue("@From_category", faker.PickRandom(categoryIds));
                        command.Parameters.AddWithValue("@To_category", faker.PickRandom(categoryIds));
                        command.Parameters.AddWithValue("@Amount", faker.Random.Decimal(1, 1000));
                        command.Parameters.AddWithValue("@Date", faker.Date.Past());
                        command.Parameters.AddWithValue("@Details", faker.Lorem.Sentence());
                        command.Parameters.AddWithValue("@Planned", faker.Random.Bool() ? 1 : 0);

                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public void Fill_Tables()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            string? connectionString = configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    Guid[] userIds = FillUsersTable(connection, this.numberOfUsers);

                    Guid[] categoryIds = FillCategoryTable(connection, this.numberOfCategories);

                    FillTransactionsTable(connection, userIds, categoryIds, 30, 50);

                    Console.WriteLine("Данi успiшно доданi до бази даних.");
                    Console.WriteLine("Натиснiть будь-яку клавiшу для друку даних з таблиць...");
                    Console.ReadKey();
                    this.Print_Tables(connection);
                    Console.WriteLine(string.Empty);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Помилка: " + ex.Message);
            }
        }

        public static void AddUserCategories(Guid userId)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            string? connectionString = configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("", connection))
                {
                    command.CommandText = "INSERT INTO Categories (UserId, Title, IsGeneral, PercentageAmount, Type) " +
                                          "VALUES (@UserId, @Title, @IsGeneral, @PercentageAmount, @Type)";

                    // Inserting the first category
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@Title", "Їжа");
                    command.Parameters.AddWithValue("@IsGeneral", 0);
                    command.Parameters.AddWithValue("@PercentageAmount", 20);
                    command.Parameters.AddWithValue("@Type", TransType.Витрати.ToString());
                    command.ExecuteNonQuery(); // Execute the query

                    // Inserting the second category
                    command.Parameters.Clear(); // Clear parameters before reusing
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@Title", "Одяг");
                    command.Parameters.AddWithValue("@IsGeneral", 0);
                    command.Parameters.AddWithValue("@PercentageAmount", 10);
                    command.Parameters.AddWithValue("@Type", TransType.Витрати.ToString());
                    command.ExecuteNonQuery(); // Execute the query

                    // Inserting the third category
                    command.Parameters.Clear(); // Clear parameters before reusing
                    command.Parameters.AddWithValue("@UserId", userId);
                    command.Parameters.AddWithValue("@Title", "Комунальні послуги");
                    command.Parameters.AddWithValue("@IsGeneral", 0);
                    command.Parameters.AddWithValue("@PercentageAmount", 10);
                    command.Parameters.AddWithValue("@Type", TransType.Витрати.ToString());
                    command.ExecuteNonQuery(); // Execute the query
                }
            }
        }

        public void Print_Tables(SqlConnection connection)
        {
            Console.WriteLine("\nДанi з таблицi користувачів:");
            DisplayDataFromTable(connection, "users");

            Console.WriteLine("\nДанi з таблицi транзакцій:");
            DisplayDataFromTable(connection, "transactions");

            Console.WriteLine("\nДанi з таблицi категорій:");
            DisplayDataFromTable(connection, "categories");

            Console.WriteLine("Натиснiть будь-яку клавiшу для завершення...");
            Console.ReadKey();
        }
    }
}
