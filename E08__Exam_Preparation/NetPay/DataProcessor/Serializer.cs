namespace NetPay.DataProcessor
{
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;

    using Data;
    using Data.Models.Enums;
    using ExportDtos;
    using Utilities;

    using static Common.EntityValidationConstants.Expense;

    public class Serializer
    {
        public static string ExportHouseholdsWhichHaveExpensesToPay(NetPayContext context)
        {
            const string xmlRootName = "Households";

            ExportHouseholdExpensesDto[] households = context
                .Households
                .Include(h => h.Expenses)
                .ThenInclude(e => e.Service)
                .Where(h => h.Expenses.Any(e => e.PaymentStatus != PaymentStatus.Paid))
                .OrderBy(h => h.ContactPerson)
                .ToArray()
                .Select(h => new ExportHouseholdExpensesDto()
                {
                    ContactPerson = h.ContactPerson,
                    Email = h.Email,
                    PhoneNumber = h.PhoneNumber,
                    Expenses = h.Expenses
                        .Where(e => e.PaymentStatus != PaymentStatus.Paid)
                        .Select(e => new ExportExpenseDto()
                        {
                            ExpenseName = e.ExpenseName,
                            Amount = e.Amount.ToString("F2"),
                            PaymentDate = e.DueDate.ToString(DueDateFormat),
                            ServiceName = e.Service.ServiceName
                        })
                        .OrderBy(e => e.PaymentDate)
                        .ThenBy(e => e.Amount)
                        .ToArray()
                })
                .ToArray();

            string result = XmlHelper
                .Serialize(households, xmlRootName);
            return result;
        }

        public static string ExportAllServicesWithSuppliers(NetPayContext context)
        {
            var services = context
                .Services
                .Select(s => new
                {
                    s.ServiceName,
                    Suppliers = s.SuppliersServices
                        .Select(ss => ss.Supplier)
                        .Select(sup => new
                        {
                            sup.SupplierName,
                        })
                        .OrderBy(sup => sup.SupplierName)
                        .ToArray(),
                })
                .OrderBy(s => s.ServiceName)
                .ToArray();

            string result = JsonConvert
                .SerializeObject(services, Formatting.Indented);
            return result;
        }
    }
}
