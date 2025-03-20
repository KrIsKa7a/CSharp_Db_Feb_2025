namespace NetPay.DataProcessor.ImportDtos
{
    using System.ComponentModel.DataAnnotations;

    using Newtonsoft.Json;

    using static Common.EntityValidationConstants.Expense;

    public class ImportExpenseDto
    {
        [Required]
        [MinLength(ExpenseNameMinLength)]
        [MaxLength(ExpenseNameMaxLength)]
        [JsonProperty(nameof(ExpenseName))]
        public string ExpenseName { get; set; } = null!;

        [Required]
        [Range(typeof(decimal), AmountMinValueStr, AmountMaxValueStr)]
        [JsonProperty(nameof(Amount))]
        public decimal Amount { get; set; }

        [Required]
        [JsonProperty(nameof(DueDate))]
        public string DueDate { get; set; } = null!;

        [Required]
        [JsonProperty(nameof(PaymentStatus))]
        public string PaymentStatus { get; set; } = null!;

        [Required]
        [JsonProperty(nameof(HouseholdId))]
        public int HouseholdId { get; set; }

        [Required]
        [JsonProperty(nameof(ServiceId))]
        public int ServiceId { get; set; }
    }
}
