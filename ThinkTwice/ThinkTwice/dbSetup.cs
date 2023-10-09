using System;
using System.Data.SqlClient;
using System.IO;

namespace DB_Setup
{
    public class SetupScript
    {
        public void SetupDatabase()
        {
            // Отримання рядка підключення до бази даних з appsettings.json
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            string connectionString = configuration.GetConnectionString("MasterConnection");
            try
            {
                // Читання SQL-скрипта з файлу
                string script = File.ReadAllText("scriptDB.sql");

                // Встановлення з'єднання з SQL Server
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Виконання SQL-скрипта для створення бази даних
                    using (SqlCommand createDbCommand = new SqlCommand("CREATE DATABASE ThinkTwice;", connection))
                    {
                        createDbCommand.ExecuteNonQuery();
                        Console.WriteLine("Базу даних 'ThinkTwice' створено успiшно.");
                    }

                    // Закриття з'єднання
                    connection.Close();
                }

                // Оновлення рядка підключення для нової бази даних
                connectionString = configuration.GetConnectionString("DefaultConnection");

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Виконання SQL-скрипта для створення таблиць у новій базі даних
                    using (SqlCommand command = new SqlCommand(script, connection))
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("Таблиці створено успішно.");
                    }

                    // Закриття з'єднання
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Помилка: " + ex.Message);
            }
        }
    }
}
