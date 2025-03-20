namespace NetPay
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;

    using Data;

    public class StartUp
    {
        static void Main(string[] args)
        {
            using NetPayContext context = new NetPayContext();

            ResetDatabase(context, shouldDropDatabase: true);

            object projectDir = GetProjectDirectory();

            ImportEntities(context, projectDir + @"Datasets/", projectDir + @"ImportResults/");
            ExportEntities(context, projectDir + @"ExportResults/");

            using IDbContextTransaction transaction = context.Database.BeginTransaction();
            transaction.Rollback();
        }

        private static void ImportEntities(NetPayContext context, string baseDir, string exportDir)
        {
            string households = DataProcessor.Deserializer
                .ImportHouseholds(context, File.ReadAllText(baseDir + "households.xml"));

            PrintAndExportEntityToFile(households, exportDir + "Actual-Result-ImportHouseholds.txt");

            string expences = DataProcessor.Deserializer
                .ImportExpenses(context, File.ReadAllText(baseDir + "expences.json"));

            PrintAndExportEntityToFile(expences, exportDir + "Actual-Result-ImportExpenses.txt");
        }

        private static void ExportEntities(NetPayContext context, string exportDir)
        {
            string householdsHavingExpensesToPayWithAllUnpaidExpences =
                DataProcessor.Serializer.ExportHouseholdsWhichHaveExpensesToPay(context);

            Console.WriteLine(householdsHavingExpensesToPayWithAllUnpaidExpences);
            File.WriteAllText(exportDir + "Actual-Result-ExportHouseholds.xml", HouseholdsHavingExpensesToPayWithAllUnpaidExpences);

            string servicesWithSuppliers =
                DataProcessor.Serializer.ExportAllServicesWithSuppliers(context);

            Console.WriteLine(servicesWithSuppliers);
            File.WriteAllText(exportDir + "Actual-Result-ExportServicesWithSuppliers.json", ServicesWithSuppliers);
        }

        private static void PrintAndExportEntityToFile(string entityOutput, string outputPath)
        {
            Console.WriteLine(entityOutput);
            File.WriteAllText(outputPath, entityOutput.TrimEnd());
        }

        private static object GetProjectDirectory()
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            string directoryName = Path.GetFileName(currentDirectory);
            string relativePath = directoryName.StartsWith("net6.0") ? @"../../../" : string.Empty;

            return relativePath;
        }

        private static void ResetDatabase(NetPayContext context, bool shouldDropDatabase = false)
        {
            if (shouldDropDatabase)
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
            else
            {
                if (context.Database.EnsureCreated())
                {
                    return;
                }

                string disableIntegrityChecksQuery = "EXEC sp_MSforeachtable @command1='ALTER TABLE ? NOCHECK CONSTRAINT ALL'";
                context.Database.ExecuteSqlRaw(disableIntegrityChecksQuery);

                string deleteRowsQuery = "EXEC sp_MSforeachtable @command1='SET QUOTED_IDENTIFIER ON;DELETE FROM ?'";
                context.Database.ExecuteSqlRaw(deleteRowsQuery);

                string enableIntegrityChecksQuery = "EXEC sp_MSforeachtable @command1='ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL'";
                context.Database.ExecuteSqlRaw(enableIntegrityChecksQuery);

                string reseedQuery = "EXEC sp_MSforeachtable @command1='IF OBJECT_ID(''?'') IN (SELECT OBJECT_ID FROM SYS.IDENTITY_COLUMNS) DBCC CHECKIDENT(''?'', RESEED, 0)'";
                context.Database.ExecuteSqlRaw(reseedQuery);
            }
        }
    }
}
