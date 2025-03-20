namespace NetPay.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;

    using Data;
    using Data.Models;
    using Data.Models.Enums;
    using ImportDtos;
    using Newtonsoft.Json;
    using Utilities;

    using static Common.EntityValidationConstants.Expense;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data format!";
        private const string DuplicationDataMessage = "Error! Data duplicated.";
        private const string SuccessfullyImportedHousehold = "Successfully imported household. Contact person: {0}";
        private const string SuccessfullyImportedExpense = "Successfully imported expense. {0}, Amount: {1}";

        public static string ImportHouseholds(NetPayContext context, string xmlString)
        {
            const string xmlRootName = "Households";

            StringBuilder output = new StringBuilder();

            ImportHouseholdDto[]? houseDtos = XmlHelper
                .Deserialize<ImportHouseholdDto[]>(xmlString, xmlRootName);
            if (houseDtos != null && houseDtos.Length > 0)
            {
                ICollection<Household> validHouseholds = new List<Household>();
                foreach (ImportHouseholdDto houseDto in houseDtos)
                {
                    if (!IsValid(houseDto))
                    {
                        output
                            .AppendLine(ErrorMessage);
                        continue;
                    }

                    bool isAlreadyImportHousehold = context
                        .Households
                        .Any(h => h.ContactPerson == houseDto.ContactPerson ||
                                  h.Email == houseDto.Email ||
                                  h.PhoneNumber == houseDto.PhoneNumber);
                    bool isToBeImportedHousehold = validHouseholds
                        .Any(h => h.ContactPerson == houseDto.ContactPerson ||
                                  h.Email == houseDto.Email ||
                                  h.PhoneNumber == houseDto.PhoneNumber);
                    if (isAlreadyImportHousehold || isToBeImportedHousehold)
                    {
                        output
                            .AppendLine(DuplicationDataMessage);
                        continue;
                    }

                    Household houseHold = new Household()
                    {
                        ContactPerson = houseDto.ContactPerson,
                        Email = houseDto.Email,
                        PhoneNumber = houseDto.PhoneNumber,
                    };
                    validHouseholds.Add(houseHold);

                    string successMessage = string
                        .Format(SuccessfullyImportedHousehold, houseDto.ContactPerson);
                    output
                        .AppendLine(successMessage);
                }

                context.Households.AddRange(validHouseholds); // Faster than calling multiple .Add()
                context.SaveChanges();
            }

            return output.ToString().TrimEnd();
        }

        public static string ImportExpenses(NetPayContext context, string jsonString)
        {
            StringBuilder output = new StringBuilder();

            ImportExpenseDto[]? expenseDtos = JsonConvert
                .DeserializeObject<ImportExpenseDto[]>(jsonString);
            if (expenseDtos != null && expenseDtos.Length > 0)
            {
                ICollection<Expense> validExpenses = new List<Expense>();
                foreach (ImportExpenseDto expenseDto in expenseDtos)
                {
                    if (!IsValid(expenseDto))
                    {
                        output
                            .AppendLine(ErrorMessage);
                        continue;
                    }

                    bool householdExists = context
                        .Households
                        .Any(h => h.Id == expenseDto.HouseholdId);
                    bool serviceExists = context
                        .Services
                        .Any(s => s.Id == expenseDto.ServiceId);
                    if ((!householdExists) || (!serviceExists))
                    {
                        output
                            .AppendLine(ErrorMessage);
                        continue;
                    }

                    bool isDueDateValid = DateTime
                        .TryParseExact(expenseDto.DueDate, DueDateFormat, CultureInfo.InvariantCulture,
                            DateTimeStyles.None, out DateTime dueDate);
                    bool isPaymentStatusValid = Enum
                        .TryParse<PaymentStatus>(expenseDto.PaymentStatus, out PaymentStatus paymentStatus);
                    if ((!isDueDateValid) || (!isPaymentStatusValid))
                    {
                        output
                            .AppendLine(ErrorMessage);
                        continue;
                    }

                    Expense expense = new Expense()
                    {
                        ExpenseName = expenseDto.ExpenseName,
                        Amount = expenseDto.Amount,
                        DueDate = dueDate,
                        PaymentStatus = paymentStatus,
                        HouseholdId = expenseDto.HouseholdId,
                        ServiceId = expenseDto.ServiceId,
                    };
                    validExpenses.Add(expense);

                    string successMessage = string
                        .Format(SuccessfullyImportedExpense, expenseDto.ExpenseName, expenseDto.Amount.ToString("F2"));
                    output
                        .AppendLine(successMessage);
                }

                context.Expenses.AddRange(validExpenses);
                context.SaveChanges();
            }

            return output.ToString().TrimEnd();
        }

        public static bool IsValid(object dto)
        {
            ValidationContext validationContext = new ValidationContext(dto);
            List<ValidationResult> validationResults = new List<ValidationResult>();

            bool isValid = Validator
                .TryValidateObject(dto, validationContext, validationResults, true);

            return isValid;
        }
    }
}
