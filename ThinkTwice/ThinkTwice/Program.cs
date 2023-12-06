using DB_Setup;

public class Program
{
    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        DbSetup setupScript = new DbSetup();
        setupScript.SetupDatabase();

        var tables = new DB_Setup.Tables();

        tables.Fill_Tables();

        var host = new WebHostBuilder()
            .UseKestrel()
            .UseStartup<Startup>()
            .Build();

        host.Run();
    }
}
