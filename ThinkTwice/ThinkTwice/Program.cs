using System;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using DB_Setup;
using System.Globalization;

class Program
{
    static void Main()
    {
        CultureInfo cultureInfo = new CultureInfo("uk-UA");
        SetupScript setupScript = new SetupScript();
        setupScript.SetupDatabase();

        // Створення об'єкту класу Tables
        var tables = new DB_Setup.Tables();

        // Виклик методу для заповнення таблиць тестовими даними
        tables.Fill_Tables();


        // Запуск ASP.NET Core додатку
        var host = new WebHostBuilder()
            .UseKestrel()
            .UseStartup<Startup>() // Вказуємо використовувати Startup клас
            .Build();

        host.Run();
    }
}
