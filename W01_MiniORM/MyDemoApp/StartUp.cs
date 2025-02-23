namespace MyDemoApp
{
    internal class StartUp
    {
        static void Main(string[] args)
        {
            string connectionString =
                @"Server=.;Database=SoftUni;Trusted_Connection=True;Encrypt=False;";
            SoftUniDbContext dbContext = new SoftUniDbContext(connectionString);

            Console.WriteLine("Entities mapped successfully!");
        }
    }
}
