namespace CarDealer
{
    using Microsoft.EntityFrameworkCore;

    using Data;

    public class StartUp
    {
        public static void Main()
        {
            using CarDealerContext dbContext = new CarDealerContext();
            dbContext.Database.Migrate();

            Console.WriteLine("Database migrated to the latest version successfully!");
        }
    }
}